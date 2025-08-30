// ***********************************************************************
// <copyright file="MySqlDialectProviderBase.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Common;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite.MySql;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using global::MySql.Data.MySqlClient;

using ServiceStack.OrmLite.MySql.Converters;
using ServiceStack.OrmLite.MySql.DataAnnotations;

/// <summary>
/// Class MySqlDialectProviderBase.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteDialectProviderBase{TDialect}" />
/// </summary>
/// <typeparam name="TDialect">The type of the t dialect.</typeparam>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteDialectProviderBase{TDialect}" />
public abstract class MySqlDialectProviderBase<TDialect> : OrmLiteDialectProviderBase<TDialect>
    where TDialect : IOrmLiteDialectProvider
{
    /// <summary>
    /// The text column definition
    /// </summary>
    private const string TextColumnDefinition = "TEXT";

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlDialectProviderBase{TDialect}" /> class.
    /// </summary>
    public MySqlDialectProviderBase()
    {
        base.AutoIncrementDefinition = "AUTO_INCREMENT";
        base.DefaultValueFormat = " DEFAULT {0}";
        base.SelectIdentitySql = "SELECT LAST_INSERT_ID()";
        base.QuoteChar = '`';

        base.InitColumnTypeMap();

        base.RegisterConverter<string>(new MySqlStringConverter());
        base.RegisterConverter<char[]>(new MySqlCharArrayConverter());
        base.RegisterConverter<bool>(new MySqlBoolConverter());

        base.RegisterConverter<byte>(new MySqlByteConverter());
        base.RegisterConverter<sbyte>(new MySqlSByteConverter());
        base.RegisterConverter<short>(new MySqlInt16Converter());
        base.RegisterConverter<ushort>(new MySqlUInt16Converter());
        base.RegisterConverter<int>(new MySqlInt32Converter());
        base.RegisterConverter<uint>(new MySqlUInt32Converter());

        base.RegisterConverter<decimal>(new MySqlDecimalConverter());

        base.RegisterConverter<Guid>(new MySqlGuidConverter());
        base.RegisterConverter<DateTimeOffset>(new MySqlDateTimeOffsetConverter());

        this.Variables = new Dictionary<string, string> {
            { OrmLiteVariables.SystemUtc, "CURRENT_TIMESTAMP" },
            { OrmLiteVariables.MaxText, "LONGTEXT" },
            { OrmLiteVariables.MaxTextUnicode, "LONGTEXT" },
            { OrmLiteVariables.True, this.SqlBool(true) },
            { OrmLiteVariables.False, this.SqlBool(false) }
        };
    }

    /// <summary>
    /// Gets a value indicating whether [supports schema].
    /// </summary>
    /// <value><c>true</c> if [supports schema]; otherwise, <c>false</c>.</value>
    public override bool SupportsSchema => false;

    /// <summary>
    /// The row version trigger format
    /// </summary>
    private const string RowVersionTriggerFormat = "{0}RowVersionUpdateTrigger";

    /// <summary>
    /// The reserved words
    /// </summary>
    public static HashSet<string> ReservedWords = new([
        "ACCESSIBLE",
        "ADD",
        "ALL",
        "ALTER",
        "ANALYZE",
        "AND",
        "AS",
        "ASC",
        "ASENSITIVE",
        "BEFORE",
        "BETWEEN",
        "BIGINT",
        "BINARY",
        "BLOB",
        "BOTH",
        "BY",
        "CALL",
        "CASCADE",
        "CASE",
        "CHANGE",
        "CHAR",
        "CHARACTER",
        "CHECK",
        "COLLATE",
        "COLUMN",
        "CONDITION",
        "CONSTRAINT",
        "CONTINUE",
        "CONVERT",
        "CREATE",
        "CROSS",
        "CUBE",
        "CUME_DIST",
        "CURRENT_DATE",
        "CURRENT_TIME",
        "CURRENT_TIMESTAMP",
        "CURRENT_USER",
        "CURSOR",
        "DATABASE",
        "DATABASES",
        "DAY_HOUR",
        "DAY_MICROSECOND",
        "DAY_MINUTE",
        "DAY_SECOND",
        "DEC",
        "DECIMAL",
        "DECLARE",
        "DEFAULT",
        "DELAYED",
        "DELETE",
        "DENSE_RANK",
        "DESC",
        "DESCRIBE",
        "DETERMINISTIC",
        "DISTINCT",
        "DISTINCTROW",
        "DIV",
        "DOUBLE",
        "DROP",
        "DUAL",
        "EACH",
        "ELSE",
        "ELSEIF",
        "EMPTY",
        "ENCLOSED",
        "ESCAPED",
        "EXCEPT",
        "EXISTS",
        "EXIT",
        "EXPLAIN",
        "FALSE",
        "FETCH",
        "FIRST_VALUE",
        "FLOAT",
        "FLOAT4",
        "FLOAT8",
        "FOR",
        "FORCE",
        "FOREIGN",
        "FROM",
        "FULLTEXT",
        "FUNCTION",
        "GENERATED",
        "GET",
        "GRANT",
        "GROUP",
        "GROUPING",
        "GROUPS",
        "HAVING",
        "HIGH_PRIORITY",
        "HOUR_MICROSECOND",
        "HOUR_MINUTE",
        "HOUR_SECOND",
        "IF",
        "IGNORE",
        "IN",
        "INDEX",
        "INFILE",
        "INNER",
        "INOUT",
        "INSENSITIVE",
        "INSERT",
        "INT",
        "INT1",
        "INT2",
        "INT3",
        "INT4",
        "INT8",
        "INTEGER",
        "INTERVAL",
        "INTO",
        "IO_AFTER_GTIDS",
        "IO_BEFORE_GTIDS",
        "IS",
        "ITERATE",
        "JOIN",
        "JSON_TABLE",
        "KEY",
        "KEYS",
        "KILL",
        "LAG",
        "LAST_VALUE",
        "LEAD",
        "LEADING",
        "LEAVE",
        "LEFT",
        "LIKE",
        "LIMIT",
        "LINEAR",
        "LINES",
        "LOAD",
        "LOCALTIME",
        "LOCALTIMESTAMP",
        "LOCK",
        "LONG",
        "LONGBLOB",
        "LONGTEXT",
        "LOOP",
        "LOW_PRIORITY",
        "MASTER_BIND",
        "MASTER_SSL_VERIFY_SERVER_CERT",
        "MATCH",
        "MAXVALUE",
        "MEDIUMBLOB",
        "MEDIUMINT",
        "MEDIUMTEXT",
        "MIDDLEINT",
        "MINUTE_MICROSECOND",
        "MINUTE_SECOND",
        "MOD",
        "MODIFIES",
        "NATURAL",
        "NOT",
        "NO_WRITE_TO_BINLOG",
        "NTH_VALUE",
        "NTILE",
        "NULL",
        "NUMERIC",
        "OF",
        "ON",
        "OPTIMIZE",
        "OPTIMIZER_COSTS",
        "OPTION",
        "OPTIONALLY",
        "OR",
        "ORDER",
        "OUT",
        "OUTER",
        "OUTFILE",
        "OVER",
        "PARTITION",
        "PERCENT_RANK",
        "PERSIST",
        "PERSIST_ONLY",
        "PRECISION",
        "PRIMARY",
        "PROCEDURE",
        "PURGE",
        "RANGE",
        "RANK",
        "READ",
        "READS",
        "READ_WRITE",
        "REAL",
        "RECURSIVE",
        "REFERENCES",
        "REGEXP",
        "RELEASE",
        "RENAME",
        "REPEAT",
        "REPLACE",
        "REQUIRE",
        "RESIGNAL",
        "RESTRICT",
        "RETURN",
        "REVOKE",
        "RIGHT",
        "RLIKE",
        "ROW",
        "ROWS",
        "ROW_NUMBER",
        "SCHEMA",
        "SCHEMAS",
        "SECOND_MICROSECOND",
        "SELECT",
        "SENSITIVE",
        "SEPARATOR",
        "SET",
        "SHOW",
        "SIGNAL",
        "SMALLINT",
        "SPATIAL",
        "SPECIFIC",
        "SQL",
        "SQLEXCEPTION",
        "SQLSTATE",
        "SQLWARNING",
        "SQL_BIG_RESULT",
        "SQL_CALC_FOUND_ROWS",
        "SQL_SMALL_RESULT",
        "SSL",
        "STARTING",
        "STORED",
        "STRAIGHT_JOIN",
        "SYSTEM",
        "TABLE",
        "TERMINATED",
        "THEN",
        "TINYBLOB",
        "TINYINT",
        "TINYTEXT",
        "TO",
        "TRAILING",
        "TRIGGER",
        "TRUE",
        "UNDO",
        "UNION",
        "UNIQUE",
        "UNLOCK",
        "UNSIGNED",
        "UPDATE",
        "USAGE",
        "USE",
        "USING",
        "UTC_DATE",
        "UTC_TIME",
        "UTC_TIMESTAMP",
        "VALUES",
        "VARBINARY",
        "VARCHAR",
        "VARCHARACTER",
        "VARYING",
        "VIRTUAL",
        "WHEN",
        "WHERE",
        "WHILE",
        "WINDOW",
        "WITH",
        "WRITE",
        "XOR",
        "YEAR_MONTH",
        "ZEROFILL"
    ], StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes the specified connection string.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    public override void Init(string connectionString)
    {
        if (connectionString.ToLower().Contains("allowloadlocalinfile=true"))
        {
            this.AllowLoadLocalInfile = true;
        }
    }

    /// <summary>
    /// Gets the load children sub select.
    /// </summary>
    /// <typeparam name="From">The type of from.</typeparam>
    /// <param name="expr">The expr.</param>
    /// <returns>System.String.</returns>
    public override string GetLoadChildrenSubSelect<From>(SqlExpression<From> expr)
    {
        // Workaround for: MySQL - This version of MySQL doesn't yet support 'LIMIT & IN/ALL/ANY/SOME subquery
        return expr.Rows != null
            ? $"SELECT * FROM ({base.GetLoadChildrenSubSelect(expr)}) AS SubQuery"
            : base.GetLoadChildrenSubSelect(expr);
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

        var triggerName = RowVersionTriggerFormat.Fmt(this.GetTableNameOnly(new TableRef(modelDef)));
        return "DROP TRIGGER IF EXISTS {0}".Fmt(this.GetQuotedName(triggerName));
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

        var triggerName = RowVersionTriggerFormat.Fmt(modelDef.ModelName);
        var triggerBody = "SET NEW.{0} = OLD.{0} + 1;".Fmt(
            modelDef.RowVersion.FieldName.SqlColumn(this));

        var sql = string.Format("CREATE TRIGGER {0} BEFORE UPDATE ON {1} FOR EACH ROW BEGIN {2} END;",
            triggerName, this.GetQuotedTableName(modelDef), triggerBody);

        return sql;
    }

    /// <summary>
    /// Quote the string so that it can be used inside an SQL-expression
    /// Escape quotes inside the string
    /// </summary>
    /// <param name="paramValue">The parameter value.</param>
    /// <returns>System.String.</returns>
    public override string GetQuotedValue(string paramValue)
    {
        return "'" + paramValue.Replace("\\", "\\\\").Replace("'", @"\'") + "'";
    }

    /// <summary>
    /// Gets the quoted value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="fieldType">Type of the field.</param>
    /// <returns>System.String.</returns>
    public override string GetQuotedValue(object value, Type fieldType)
    {
        if (value == null)
        {
            return "NULL";
        }

        if (fieldType == typeof(byte[]))
        {
            return "0x" + BitConverter.ToString((byte[])value).Replace("-", "");
        }

        return base.GetQuotedValue(value, fieldType);
    }

    public override string UnquotedTable(TableRef tableRef)
    {
        if (tableRef.QuotedName != null)
        {
            return tableRef.QuotedName.Replace("\"", "");
        }

        var alias = tableRef.ModelDef?.Alias;
        if (alias != null)
        {
            return tableRef.ModelDef?.Schema != null
                ? this.NamingStrategy.GetSchemaName(tableRef.ModelDef.Schema) + "_" + this.NamingStrategy.GetTableAlias(alias)
                : this.NamingStrategy.GetTableAlias(alias);
        }

        var schema = tableRef.ModelDef?.Schema ?? tableRef.Schema;
        var tableName = tableRef.ModelDef?.Name ?? tableRef.Name;
        return schema != null
            ? this.NamingStrategy.GetSchemaName(schema) + "_" + this.NamingStrategy.GetTableName(tableName)
            : this.NamingStrategy.GetTableName(tableName);
    }

    public override string QuoteSchema(string schema, string table)
    {
        return this.GetQuotedName(this.JoinSchema(schema, table));
    }

    public override string JoinSchema(string schema, string table)
    {
        return string.IsNullOrEmpty(schema) || table.StartsWith(schema + "_")
            ? table
            : schema + "_" + table;
    }

    /// <summary>
    /// Shoulds the quote.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool ShouldQuote(string name)
    {
        return name != null &&
               (ReservedWords.Contains(name) || name.Contains(' ') ||
                name.Contains('.'));
    }

    /// <summary>
    /// SQLs the expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public override SqlExpression<T> SqlExpression<T>()
    {
        return new MySqlExpression<T>(this);
    }

    /// <summary>
    /// Converts to tablenamesstatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    public override string ToTableNamesStatement(string schema)
    {
        return schema == null
            ? "SELECT table_name FROM information_schema.tables WHERE table_type='BASE TABLE' AND table_schema = DATABASE()"
            : "SELECT table_name FROM information_schema.tables WHERE table_type='BASE TABLE' AND table_schema = DATABASE() AND table_name LIKE {0}"
                .SqlFmt(this, this.NamingStrategy.GetSchemaName(schema) + "\\_%");
    }

    /// <summary>
    /// Return table, row count SQL for listing all tables with their row counts
    /// </summary>
    /// <param name="live">If true returns live current row counts of each table (slower), otherwise returns cached row counts from RDBMS table stats</param>
    /// <param name="schema">The table schema if any</param>
    /// <returns>System.String.</returns>
    public override string ToTableNamesWithRowCountsStatement(bool live, string schema)
    {
        if (live)
        {
            return null;
        }

        return schema == null
            ? "SELECT table_name, table_rows FROM information_schema.tables WHERE table_type='BASE TABLE' AND table_schema = DATABASE()"
            : "SELECT table_name, table_rows FROM information_schema.tables WHERE table_type='BASE TABLE' AND table_schema = DATABASE() AND table_name LIKE {0}"
                .SqlFmt(this, this.NamingStrategy.GetSchemaName(schema) + "\\_%");
    }

    /// <summary>
    /// Doeses the table exist.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="schema">The schema.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool DoesTableExist(IDbCommand dbCmd, TableRef tableRef)
    {
        var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0} AND TABLE_SCHEMA = {1}"
            .SqlFmt(this.UnquotedTable(tableRef), dbCmd.Connection.Database);

        var result = dbCmd.ExecLongScalar(sql);

        return result > 0;
    }

    /// <summary>
    /// Does table exist as an asynchronous operation.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
    public async override Task<bool> DoesTableExistAsync(IDbCommand dbCmd, TableRef tableRef,
        CancellationToken token = default)
    {
        var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0} AND TABLE_SCHEMA = {1}"
            .SqlFmt(this.UnquotedTable(tableRef), dbCmd.Connection.Database);

        var result = await dbCmd.ExecLongScalarAsync(sql, token);

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
    public override bool DoesColumnExist(IDbConnection db, string columnName, TableRef tableRef)
    {
        var tableName = this.UnquotedTable(tableRef);

        var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS"
                  + " WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName AND TABLE_SCHEMA = @schema"
                      .SqlFmt(this.UnquotedTable(tableRef), columnName);

        var result = db.SqlScalar<long>(sql, new { tableName, columnName, schema = db.Database });

        return result > 0;
    }

    /// <summary>
    /// Does column exist as an asynchronous operation.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
    public async override Task<bool> DoesColumnExistAsync(IDbConnection db, string columnName, TableRef tableRef, CancellationToken token = default)
    {
        var tableName = this.UnquotedTable(tableRef);
        var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS"
                  + " WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName AND TABLE_SCHEMA = @schema"
                      .SqlFmt(tableName, columnName);

        var result = await db.SqlScalarAsync<long>(sql, new { tableName, columnName, schema = db.Database }, token);

        return result > 0;
    }

    /// <summary>
    /// Gets the type of the column data.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnDataType(
        IDbConnection db,
        string columnName,
        TableRef tableRef)
    {
        var tableName = this.GetTableNameOnly(tableRef);
        var schema = this.GetSchemaName(tableRef);
        var sql =
            "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName"
                .SqlFmt(this, this.GetTableNameOnly(new TableRef(schema, tableName)), columnName);

        if (schema != null)
        {
            sql += " AND TABLE_SCHEMA = @schema";
        }

        return db.SqlScalar<string>(sql, new { tableName, columnName, schema });
    }

    /// <summary>
    /// Gets the maximum length of the column.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <returns>System.Int64.</returns>
    public override long GetColumnMaxLength(
        IDbConnection db,
        string columnName,
        TableRef tableRef)
    {
        var tableName = this.GetTableNameOnly(tableRef);
        var sql =
            "SELECT character_maximum_length FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName"
                .SqlFmt(this, tableName, columnName);

        var schema = this.GetSchemaName(tableRef);
        if (schema != null)
        {
            sql += " AND TABLE_SCHEMA = @schema";
        }

        return db.SqlScalar<long>(sql, new { tableName, columnName, schema });
    }

    /// <summary>
    /// Converts to createtablestatement.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <returns>System.String.</returns>
    public override string ToCreateTableStatement(Type tableType)
    {
        var sbColumns = StringBuilderCache.Allocate();
        var sbConstraints = StringBuilderCache.Allocate();

        var modelDef = GetModel(tableType);
        foreach (var fieldDef in this.CreateTableFieldsStrategy(modelDef))
        {
            if (fieldDef.CustomSelect != null || fieldDef.IsComputed && !fieldDef.IsPersisted)
            {
                continue;
            }

            if (sbColumns.Length != 0)
            {
                sbColumns.Append(", \n  ");
            }

            sbColumns.Append(this.GetColumnDefinition(fieldDef, modelDef));

            var sqlConstraint = this.GetCheckConstraint(modelDef, fieldDef);
            if (sqlConstraint != null)
            {
                sbConstraints.Append(",\n" + sqlConstraint);
            }

            if (fieldDef.ForeignKey == null || OrmLiteConfig.SkipForeignKeys)
            {
                continue;
            }

            var refModelDef = GetModel(fieldDef.ForeignKey.ReferenceType);
            sbConstraints.Append(
                $", \n\n  CONSTRAINT {this.GetQuotedName(fieldDef.ForeignKey.GetForeignKeyName(modelDef, refModelDef, this.NamingStrategy, fieldDef))} FOREIGN KEY ({this.GetQuotedColumnName(fieldDef)}) REFERENCES {this.GetQuotedTableName(refModelDef)} ({this.GetQuotedColumnName(refModelDef.PrimaryKey)})");

            if (!string.IsNullOrEmpty(fieldDef.ForeignKey.OnDelete))
            {
                sbConstraints.Append($" ON DELETE {fieldDef.ForeignKey.OnDelete}");
            }

            if (!string.IsNullOrEmpty(fieldDef.ForeignKey.OnUpdate))
            {
                sbConstraints.Append($" ON UPDATE {fieldDef.ForeignKey.OnUpdate}");
            }
        }

        var uniqueConstraints = this.GetUniqueConstraints(modelDef);
        if (uniqueConstraints != null)
        {
            sbConstraints.Append(",\n" + uniqueConstraints);
        }

        if (modelDef.CompositePrimaryKeys.Count != 0)
        {
            sbConstraints.Append(",\n");

            sbConstraints.Append(" PRIMARY KEY (");

            sbConstraints.Append(
                modelDef.CompositePrimaryKeys[0].FieldNames.Map(f => modelDef.GetQuotedName(f, this))
                    .Join(","));

            sbConstraints.Append(") ");
        }

        var sql =
            $"CREATE TABLE {this.GetQuotedTableName(modelDef)} \n(\n  {StringBuilderCache.ReturnAndFree(sbColumns)}{StringBuilderCacheAlt.ReturnAndFree(sbConstraints)} \n); \n";

        return sql;
    }

    /// <summary>
    /// Gets the schemas.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public override List<string> GetSchemas(IDbCommand dbCmd)
    {
        var sql =
            "SELECT DISTINCT TABLE_SCHEMA FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA NOT IN ('information_schema', 'performance_schema', 'sys', 'mysql')";
        return dbCmd.SqlColumn<string>(sql);
    }

    /// <summary>
    /// Gets the schema tables.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>Dictionary&lt;System.String, List&lt;System.String&gt;&gt;.</returns>
    public override Dictionary<string, List<string>> GetSchemaTables(IDbCommand dbCmd)
    {
        var sql =
            "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA NOT IN ('information_schema', 'performance_schema', 'sys', 'mysql')";
        return dbCmd.Lookup<string, string>(sql);
    }

    /// <summary>
    /// Doeses the schema exist.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="schemaName">Name of the schema.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool DoesSchemaExist(IDbCommand dbCmd, string schemaName)
    {
        // schema is prefixed to table name
        return true;
    }

    /// <summary>
    /// Converts to createschemastatement.
    /// </summary>
    /// <param name="schemaName">Name of the schema.</param>
    /// <returns>System.String.</returns>
    public override string ToCreateSchemaStatement(string schemaName)
    {
        // https://mariadb.com/kb/en/library/create-database/
        return "SELECT 1";
    }

    /// <summary>
    /// Converts to dropforeignkeystatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <param name="foreignKeyName">Name of the foreign key.</param>
    /// <returns>string.</returns>
    public override string ToDropForeignKeyStatement(TableRef tableRef, string foreignKeyName)
    {
        return
            $"ALTER TABLE {this.QuoteTable(tableRef)} DROP FOREIGN KEY {this.GetQuotedName(foreignKeyName)};";
    }

    /// <summary>
    /// Create Drop Index statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="indexName">Name of the index.</param>
    /// <returns>System.String.</returns>
    public override string ToDropIndexStatement<T>(string indexName)
    {
        return $"DROP INDEX {this.GetQuotedName(indexName)} ON {this.GetQuotedTableName(typeof(T))}";
    }

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnDefinition(FieldDefinition fieldDef)
    {
        if (fieldDef.PropertyInfo?.HasAttributeCached<TextAttribute>() == true)
        {
            var sql = StringBuilderCache.Allocate();
            sql.Append(
                $"{this.GetQuotedName(this.NamingStrategy.GetColumnName(fieldDef.FieldName))} {TextColumnDefinition}");
            sql.Append(fieldDef.IsNullable ? " NULL" : " NOT NULL");
            return StringBuilderCache.ReturnAndFree(sql);
        }

        var ret = base.GetColumnDefinition(fieldDef);

        return fieldDef.IsRowVersion ? $"{ret} DEFAULT 1" : ret;
    }

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnDefinition(FieldDefinition fieldDef, ModelDefinition modelDef)
    {
        if (fieldDef.PropertyInfo?.HasAttributeCached<TextAttribute>() == true)
        {
            var sql = StringBuilderCache.Allocate();
            sql.Append(
                $"{this.GetQuotedName(this.NamingStrategy.GetColumnName(fieldDef.FieldName))} {TextColumnDefinition}");
            sql.Append(fieldDef.IsNullable ? " NULL" : " NOT NULL");
            return StringBuilderCache.ReturnAndFree(sql);
        }

        var ret = base.GetColumnDefinition(fieldDef, modelDef);

        return fieldDef.IsRowVersion ? $"{ret} DEFAULT 1" : ret;
    }

    /// <summary>
    /// SQLs the conflict.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="conflictResolution">The conflict resolution.</param>
    /// <returns>System.String.</returns>
    public override string SqlConflict(string sql, string conflictResolution)
    {
        var parts = sql.SplitOnFirst(' ');
        return $"{parts[0]} {conflictResolution} {parts[1]}";
    }

    /// <summary>
    /// SQLs the currency.
    /// </summary>
    /// <param name="fieldOrValue">The field or value.</param>
    /// <param name="currencySymbol">The currency symbol.</param>
    /// <returns>System.String.</returns>
    public override string SqlCurrency(string fieldOrValue, string currencySymbol)
    {
        return this.SqlConcat([$"'{currencySymbol}'", $"cast({fieldOrValue} as decimal(15,2))"]);
    }

    /// <summary>
    /// SQLs the cast.
    /// </summary>
    /// <param name="fieldOrValue">The field or value.</param>
    /// <param name="castAs">The cast as.</param>
    /// <returns>System.String.</returns>
    public override string SqlCast(object fieldOrValue, string castAs)
    {
        return castAs == Sql.VARCHAR
            ? $"CAST({fieldOrValue} AS CHAR(1000))"
            : $"CAST({fieldOrValue} AS {castAs})";
    }

    /// <summary>
    /// SQLs the bool.
    /// </summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
    /// <returns>System.String.</returns>
    public override string SqlBool(bool value)
    {
        return value ? "1" : "0";
    }

    /// <summary>
    /// Enables the foreign keys check.
    /// </summary>
    /// <param name="cmd">The command.</param>
    public override void EnableForeignKeysCheck(IDbCommand cmd)
    {
        cmd.ExecNonQuery("SET FOREIGN_KEY_CHECKS=1;");
    }

    /// <summary>
    /// Enables the foreign keys check asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public override Task EnableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default)
    {
        return cmd.ExecNonQueryAsync("SET FOREIGN_KEY_CHECKS=1;", null, token);
    }

    /// <summary>
    /// Disables the foreign keys check.
    /// </summary>
    /// <param name="cmd">The command.</param>
    public override void DisableForeignKeysCheck(IDbCommand cmd)
    {
        cmd.ExecNonQuery("SET FOREIGN_KEY_CHECKS=0;");
    }

    /// <summary>
    /// Disables the foreign keys check asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public override Task DisableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default)
    {
        return cmd.ExecNonQueryAsync("SET FOREIGN_KEY_CHECKS=0;", null, token);
    }

    /// <summary>
    /// Unwraps the specified database.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <returns>DbConnection.</returns>
    protected DbConnection Unwrap(IDbConnection db)
    {
        return (DbConnection)db.ToDbConnection();
    }

    /// <summary>
    /// Unwraps the specified command.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <returns>DbCommand.</returns>
    protected DbCommand Unwrap(IDbCommand cmd)
    {
        return (DbCommand)cmd.ToDbCommand();
    }

    /// <summary>
    /// Unwraps the specified reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>DbDataReader.</returns>
    protected DbDataReader Unwrap(IDataReader reader)
    {
        return (DbDataReader)reader;
    }

    /// <summary>
    /// Gets a value indicating whether [supports asynchronous].
    /// </summary>
    /// <value><c>true</c> if [supports asynchronous]; otherwise, <c>false</c>.</value>
    public override bool SupportsAsync => true;

    /// <summary>
    /// Opens the asynchronous.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public override Task OpenAsync(IDbConnection db, CancellationToken token = default)
    {
        return this.Unwrap(db).OpenAsync(token);
    }

    /// <summary>
    /// Executes the reader asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;IDataReader&gt;.</returns>
    public override Task<IDataReader> ExecuteReaderAsync(IDbCommand cmd, CancellationToken token = default)
    {
        return this.Unwrap(cmd).ExecuteReaderAsync(token).Then(x => (IDataReader)x);
    }

    /// <summary>
    /// Executes the non query asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    public override Task<int> ExecuteNonQueryAsync(IDbCommand cmd, CancellationToken token = default)
    {
        return this.Unwrap(cmd).ExecuteNonQueryAsync(token);
    }

    /// <summary>
    /// Executes the scalar asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Object&gt;.</returns>
    public override Task<object> ExecuteScalarAsync(IDbCommand cmd, CancellationToken token = default)
    {
        return this.Unwrap(cmd).ExecuteScalarAsync(token);
    }

    /// <summary>
    /// Reads the asynchronous.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public override Task<bool> ReadAsync(IDataReader reader, CancellationToken token = default)
    {
        return this.Unwrap(reader).ReadAsync(token);
    }

    /// <summary>
    /// Readers the each.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="fn">The function.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
    public async override Task<List<T>> ReaderEach<T>(IDataReader reader, Func<T> fn, CancellationToken token = default)
    {
        try
        {
            var to = new List<T>();
            while (await this.ReadAsync(reader, token).ConfigureAwait(false))
            {
                var row = fn();
                to.Add(row);
            }

            return to;
        }
        finally
        {
            reader.Dispose();
        }
    }

    /// <summary>
    /// Readers the each.
    /// </summary>
    /// <typeparam name="Return">The type of the return.</typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="fn">The function.</param>
    /// <param name="source">The source.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;Return&gt;.</returns>
    public async override Task<Return> ReaderEach<Return>(IDataReader reader, Action fn, Return source,
        CancellationToken token = default)
    {
        try
        {
            while (await this.ReadAsync(reader, token).ConfigureAwait(false))
            {
                fn();
            }

            return source;
        }
        finally
        {
            reader.Dispose();
        }
    }

    /// <summary>
    /// Readers the read.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="fn">The function.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;T&gt;.</returns>
    public async override Task<T> ReaderRead<T>(IDataReader reader, Func<T> fn, CancellationToken token = default)
    {
        try
        {
            if (await this.ReadAsync(reader, token).ConfigureAwait(false))
            {
                return fn();
            }

            return default;
        }
        finally
        {
            reader.Dispose();
        }
    }

    /// <summary>
    /// Gets the drop function.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="functionName">Name of the function.</param>
    /// <returns>System.String.</returns>
    public override string GetDropFunction(string database, string functionName)
    {
        var sb = StringBuilderCache.Allocate();

        var tableName = $"{database}.{base.NamingStrategy.GetTableName(functionName)}";

        sb.Append("DROP FUNCTION IF EXISTS ");
        sb.Append($"{tableName}");

        sb.Append(';');

        return StringBuilderCache.ReturnAndFree(sb);
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

        var tableName = $"{database}.{base.NamingStrategy.GetTableName(modelDef.ModelName)}";

        sb.Append($"CREATE VIEW {tableName} as ");

        sb.Append(selectSql);

        sb.Append(';');

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
        sb.Append($"{tableName}");

        sb.Append(';');

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Gets the add composite primary key sql command.
    /// </summary>
    /// <param name="database">The database name.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="fieldNameA">The field name a.</param>
    /// <param name="fieldNameB">The field name b.</param>
    /// <returns>Returns the SQL Command</returns>
    public override string GetAddCompositePrimaryKey(string database, ModelDefinition modelDef, string fieldNameA,
        string fieldNameB)
    {
        var sb = StringBuilderCache.Allocate();

        var tableName = $"{database}.{base.NamingStrategy.GetTableName(modelDef.ModelName)}";

        sb.Append($"alter table {tableName}");
        sb.Append($"{this.NamingStrategy.GetTableName(modelDef)} ADD PRIMARY KEY ({fieldNameA},{fieldNameB})");

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Gets the name of the primary key.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>Returns the Primary Key Name</returns>
    public override string GetPrimaryKeyName(ModelDefinition modelDef)
    {
        // Return Empty. Doesn't have a name in MySQL
        return string.Empty;
    }

    /// <summary>
    /// Gets the drop primary key constraint.
    /// </summary>
    /// <param name="database">the database name.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public override string GetDropPrimaryKeyConstraint(string database, ModelDefinition modelDef, string name)
    {
        var sb = StringBuilderCache.Allocate();

        var tableName = $"{database}.{base.NamingStrategy.GetTableName(modelDef.ModelName)}";

        sb.Append($"alter table {tableName} drop primary key;");

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Gets the drop primary key constraint.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <param name="fieldNameA">The field name a.</param>
    /// <param name="fieldNameB">The field name b.</param>
    /// <returns>System.String.</returns>
    public override string GetDropPrimaryKeyConstraint(string database, ModelDefinition modelDef, string name,
        string fieldNameA, string fieldNameB)
    {
        var sb = StringBuilderCache.Allocate();

        var tableName = $"{database}.{base.NamingStrategy.GetTableName(modelDef.ModelName)}";

        sb.Append($"alter table {tableName} drop primary key,");

        sb.Append($" ADD PRIMARY KEY ({fieldNameA},{fieldNameB});");

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Gets the UTC date function.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string GetUtcDateFunction()
    {
        return "UTC_DATE()";
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
        return $"DATEDIFF({date1}, {date2})";
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
        return $"cast(sign({expression}) as signed)";
    }

    /// <summary>
    /// Databases the fragmentation information.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>System.String.</returns>
    public override string DatabaseFragmentationInfo(string database)
    {
        var sb = new StringBuilder();

        sb.AppendLine("select ENGINE,");
        sb.AppendLine("concat(TABLE_SCHEMA, '.', TABLE_NAME) as table_name, ");
        sb.AppendLine("round(DATA_LENGTH/1024/1024, 2) as data_length,");
        sb.AppendLine("round(INDEX_LENGTH/1024/1024, 2) as index_length,");
        sb.AppendLine("round(DATA_FREE/1024/1024, 2) as data_free,");
        sb.AppendLine("(data_free/(index_length+data_length)) as frag_ratio");
        sb.AppendLine("FROM information_schema.tables");
        sb.AppendLine($"WHERE table_schema = '{database}'");
        sb.AppendLine("ORDER BY frag_ratio DESC;");

        return sb.ToString();
    }

    /// <summary>
    /// Databases the size.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>System.String.</returns>
    public override string DatabaseSize(string database)
    {
        return
            $"SELECT sum( data_length + index_length ) / 1024 / 1024 FROM information_schema.TABLES WHERE table_schema = '{database}'";
    }

    /// <summary>
    /// SQLs the version.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string SQLVersion()
    {
        return "select @@version";
    }

    /// <summary>
    /// SQLs the name of the server.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string SQLServerName()
    {
        return "MySQL";
    }

    /// <summary>
    /// Shrinks the database.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>System.String.</returns>
    public override string ShrinkDatabase(string database)
    {
        return ""; //$"optimize table {database}";
    }

    /// <summary>
    /// Res the index database.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="objectQualifier">The object qualifier.</param>
    /// <returns>System.String.</returns>
    public override string ReIndexDatabase(string database, string objectQualifier)
    {
        return ""; //$"REINDEX DATABASE {database}";
    }

    /// <summary>
    /// Changes the recovery mode.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="mode">The mode.</param>
    /// <returns>System.String.</returns>
    public override string ChangeRecoveryMode(string database, string mode)
    {
        return "";
    }

    /// <summary>
    /// Just runs the SQL command according to specifications.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>Returns the Results</returns>
    public override string InnerRunSqlExecuteReader(IDbCommand command)
    {
        var sqlCommand = command as MySqlCommand;

        MySqlDataReader reader = null;
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
                            results.Append(',');
                            results.Append(n);
                        });

                    results.AppendLine();

                    while (reader.Read())
                    {
                        results.Append($"{rowIndex++}");

                        // dump all columns...
                        columnNames.ForEach(
                            col => results.Append($@",""{reader[col].ToString().Replace("\"", "\"\"")}"""));

                        results.AppendLine();
                    }
                }
                else if (reader.RecordsAffected > 0)
                {
                    results.Append($"{reader.RecordsAffected} Record(s) Affected");
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
            results.Append($"SQL ERROR: {x}");
        }

        return results.ToString();
    }
}