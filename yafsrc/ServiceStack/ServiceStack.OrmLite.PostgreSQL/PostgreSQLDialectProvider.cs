// ***********************************************************************
// <copyright file="PostgreSQLDialectProvider.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.Data.Common;

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite.PostgreSQL;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Npgsql;

using NpgsqlTypes;

using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite.Converters;
using ServiceStack.OrmLite.PostgreSQL.Converters;

/// <summary>
/// Class PostgreSqlDialectProvider.
/// </summary>
public class PostgreSqlDialectProvider : OrmLiteDialectProviderBase<PostgreSqlDialectProvider>
{
    public override DbKind Kind => DbKind.PostgreSql;

    /// <summary>
    /// The instance
    /// </summary>
    public static PostgreSqlDialectProvider Instance = new();

    /// <summary>
    /// Gets or sets a value indicating whether [use returning for last insert identifier].
    /// </summary>
    /// <value><c>true</c> if [use returning for last insert identifier]; otherwise, <c>false</c>.</value>
    public bool UseReturningForLastInsertId { get; set; } = true;

    /// <summary>
    /// Gets or sets the automatic identifier unique identifier function.
    /// </summary>
    /// <value>The automatic identifier unique identifier function.</value>
    public string AutoIdGuidFunction { get; set; } = "uuid_generate_v4()";

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlDialectProvider" /> class.
    /// </summary>
    public PostgreSqlDialectProvider()
    {
        this.VectorConverter = new VectorTextConverter();
        base.AutoIncrementDefinition = "";
        base.ParamString = ":";
        base.SelectIdentitySql = "SELECT LASTVAL()";
        this.NamingStrategy = new PostgreSqlNamingStrategy();
        this.StringSerializer = new JsonStringSerializer();

        base.InitColumnTypeMap();

        this.RowVersionConverter = new PostgreSqlRowVersionConverter();

        RegisterConverter<string>(new PostgreSqlStringConverter());
        RegisterConverter<char[]>(new PostgreSqlCharArrayConverter());

        RegisterConverter<bool>(new PostgreSqlBoolConverter());
        RegisterConverter<Guid>(new PostgreSqlGuidConverter());

        RegisterConverter<DateTime>(new PostgreSqlDateTimeConverter());
        RegisterConverter<DateTimeOffset>(new PostgreSqlDateTimeOffsetConverter());


        RegisterConverter<sbyte>(new PostrgreSqlSByteConverter());
        RegisterConverter<ushort>(new PostrgreSqlUInt16Converter());
        RegisterConverter<uint>(new PostrgreSqlUInt32Converter());
        RegisterConverter<ulong>(new PostrgreSqlUInt64Converter());

        RegisterConverter<float>(new PostrgreSqlFloatConverter());
        RegisterConverter<double>(new PostrgreSqlDoubleConverter());
        RegisterConverter<decimal>(new PostrgreSqlDecimalConverter());

        RegisterConverter<byte[]>(new PostrgreSqlByteArrayConverter());

        //TODO provide support for pgsql native data structures:
        RegisterConverter<string[]>(new PostgreSqlStringArrayConverter());
        RegisterConverter<short[]>(new PostgreSqlShortArrayConverter());
        RegisterConverter<int[]>(new PostgreSqlIntArrayConverter());
        RegisterConverter<long[]>(new PostgreSqlLongArrayConverter());
        RegisterConverter<float[]>(new PostgreSqlFloatArrayConverter());
        RegisterConverter<double[]>(new PostgreSqlDoubleArrayConverter());
        RegisterConverter<decimal[]>(new PostgreSqlDecimalArrayConverter());
        RegisterConverter<DateTime[]>(new PostgreSqlDateTimeTimeStampArrayConverter());
        RegisterConverter<DateTimeOffset[]>(new PostgreSqlDateTimeOffsetTimeStampTzArrayConverter());

        RegisterConverter<XmlValue>(new PostgreSqlXmlConverter());

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.EnableLegacyCaseInsensitiveDbParameters", true);

#if NET6_0
            RegisterConverter<DateOnly>(new PostgreSqlDateOnlyConverter());
#endif

#if NET481
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
#endif

        this.Variables = new Dictionary<string, string>
                         {
                             { OrmLiteVariables.SystemUtc, "now() at time zone 'utc'" },
                             { OrmLiteVariables.MaxText, "TEXT" },
                             { OrmLiteVariables.MaxTextUnicode, "TEXT" },
                             { OrmLiteVariables.True, SqlBool(true) },
                             { OrmLiteVariables.False, SqlBool(false) }
                         };
    }

    /// <summary>
    /// Sets a value indicating whether [use hstore].
    /// </summary>
    /// <value><c>true</c> if [use hstore]; otherwise, <c>false</c>.</value>
    public bool UseHstore
    {
        set
        {
            if (value)
            {
                RegisterConverter<IDictionary<string, string>>(new PostgreSqlHstoreConverter());
                RegisterConverter<Dictionary<string, string>>(new PostgreSqlHstoreConverter());
            }
            else
            {
                RemoveConverter<IDictionary<string, string>>();
                RemoveConverter<Dictionary<string, string>>();
            }
        }
    }

    /// <summary>
    /// The normalize
    /// </summary>
    private bool normalize;
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="PostgreSqlDialectProvider" /> is normalize.
    /// </summary>
    /// <value><c>true</c> if normalize; otherwise, <c>false</c>.</value>
    public bool Normalize
    {
        get => normalize;
        set
        {
            normalize = value;
            NamingStrategy = normalize
                                 ? new OrmLiteNamingStrategyBase()
                                 : new PostgreSqlNamingStrategy();
        }
    }

    //https://www.postgresql.org/docs/7.3/static/sql-keywords-appendix.html
    /// <summary>
    /// The reserved words
    /// </summary>
    public static HashSet<string> ReservedWords = new([
        "ALL",
                                                              "ANALYSE",
                                                              "ANALYZE",
                                                              "AND",
                                                              "ANY",
                                                              "AS",
                                                              "ASC",
                                                              "AUTHORIZATION",
                                                              "BETWEEN",
                                                              "BINARY",
                                                              "BOTH",
                                                              "CASE",
                                                              "CAST",
                                                              "CHECK",
                                                              "COLLATE",
                                                              "COLUMN",
                                                              "CONSTRAINT",
                                                              "CURRENT_DATE",
                                                              "CURRENT_TIME",
                                                              "CURRENT_TIMESTAMP",
                                                              "CURRENT_USER",
                                                              "DEFAULT",
                                                              "DEFERRABLE",
                                                              "DISTINCT",
                                                              "DO",
                                                              "ELSE",
                                                              "END",
                                                              "EXCEPT",
                                                              "FOR",
                                                              "FOREIGN",
                                                              "FREEZE",
                                                              "FROM",
                                                              "FULL",
                                                              "HAVING",
                                                              "ILIKE",
                                                              "IN",
                                                              "INITIALLY",
                                                              "INNER",
                                                              "INTERSECT",
                                                              "INTO",
                                                              "IS",
                                                              "ISNULL",
                                                              "JOIN",
                                                              "LEADING",
                                                              "LEFT",
                                                              "LIKE",
                                                              "LIMIT",
                                                              "LOCALTIME",
                                                              "LOCALTIMESTAMP",
                                                              "NEW",
                                                              "NOT",
                                                              "NOTNULL",
                                                              "NULL",
                                                              "OFF",
                                                              "OFFSET",
                                                              "OLD",
                                                              "ON",
                                                              "ONLY",
                                                              "OR",
                                                              "ORDER",
                                                              "OUTER",
                                                              "OVERLAPS",
                                                              "PLACING",
                                                              "PRIMARY",
                                                              "REFERENCES",
                                                              "RIGHT",
                                                              "SELECT",
                                                              "SESSION_USER",
                                                              "SIMILAR",
                                                              "SOME",
                                                              "TABLE",
                                                              "THEN",
                                                              "TO",
                                                              "TRAILING",
                                                              "TRUE",
                                                              "UNION",
                                                              "UNIQUE",
                                                              "USER",
                                                              "USING",
                                                              "VERBOSE",
                                                              "WHEN",
                                                              "WHERE"
    ], StringComparer.OrdinalIgnoreCase);

    protected override bool SupportsIndexInclude => true;

    // PostgreSQL only has virtual generated columns from v18, so they're always stored
    protected override string GetGeneratedColumnDefinition(FieldDefinition fieldDef)
    {
        var columnType = this.GetFieldTypeDefinition(fieldDef);
        return $"{this.GetQuotedColumnName(fieldDef)} {columnType} GENERATED ALWAYS AS " +
               $"({this.ResolveColumnRefs(fieldDef.ModelDef, fieldDef.ComputeExpression)}) STORED";
    }

    public override List<string> ToCreateCommentStatements(Type tableType)
    {
        var to = new List<string>();
        var modelDef = GetModel(tableType);
        var table = this.GetQuotedTableName(modelDef);
        if (!string.IsNullOrEmpty(modelDef.Description))
        {
            to.Add($"COMMENT ON TABLE {table} IS {this.GetQuotedValue(modelDef.Description)}");
        }

        foreach (var fieldDef in modelDef.FieldDefinitions)
        {
            if (string.IsNullOrEmpty(fieldDef.Description) || fieldDef.ShouldSkipCreate() || fieldDef.IsRowVersion)
            {
                continue;
            }

            to.Add($"COMMENT ON COLUMN {table}.{this.GetQuotedColumnName(fieldDef)} IS {this.GetQuotedValue(fieldDef.Description)}");
        }
        return to;
    }

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnDefinition(FieldDefinition fieldDef)
    {
        if (fieldDef.IsRowVersion)
        {
            return null;
        }

        if (fieldDef.IsGenerated)
        {
            return this.GetGeneratedColumnDefinition(fieldDef);
        }

        string fieldDefinition = null;

        if (fieldDef.CustomFieldDefinition != null)
        {
            fieldDefinition = ResolveFragment(fieldDef.CustomFieldDefinition);
        }
        else
        {
            if (fieldDef.AutoIncrement)
            {
                if (fieldDef.ColumnType == typeof(long))
                {
                    fieldDefinition = "bigserial";
                }
                else if (fieldDef.ColumnType == typeof(int))
                {
                    fieldDefinition = "serial";
                }
            }
            else
            {
                fieldDefinition = this.GetFieldTypeDefinition(fieldDef);
            }
        }

        var sql = StringBuilderCache.Allocate();
        sql.AppendFormat("{0} {1}", GetQuotedColumnName(fieldDef.FieldName), fieldDefinition);

        if (fieldDef.IsPrimaryKey)
        {
            sql.Append(" PRIMARY KEY");
        }
        else
        {
            if (fieldDef.IsNullable)
            {
                sql.Append(" NULL");
            }
            else
            {
                sql.Append(" NOT NULL");
            }
        }

        if (fieldDef.IsUniqueConstraint)
        {
            sql.Append(" UNIQUE");
        }

        var defaultValue = GetDefaultValue(fieldDef);
        if (!string.IsNullOrEmpty(defaultValue))
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                if (fieldDef.ColumnType == typeof(bool))
                {
                    sql.AppendFormat(this.DefaultValueFormat, defaultValue == "1" ? "TRUE" : "FALSE");
                }
                else
                {
                    sql.AppendFormat(this.DefaultValueFormat, defaultValue);
                }
            }
        }

        var definition = StringBuilderCache.ReturnAndFree(sql);
        return definition;
    }

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnDefinition(FieldDefinition fieldDef, ModelDefinition modelDef)
    {
        if (fieldDef.IsGenerated)
        {
            return this.GetGeneratedColumnDefinition(fieldDef);
        }

        string fieldDefinition = null;

        if (fieldDef.CustomFieldDefinition != null)
        {
            fieldDefinition = this.ResolveFragment(fieldDef.CustomFieldDefinition);
        }
        else
        {
            if (fieldDef.AutoIncrement)
            {
                if (fieldDef.ColumnType == typeof(long))
                {
                    fieldDefinition = "bigserial";
                }
                else if (fieldDef.ColumnType == typeof(int))
                {
                    fieldDefinition = "serial";
                }
            }
            else
            {
                fieldDefinition = this.GetFieldTypeDefinition(fieldDef);
            }
        }

        var sql = StringBuilderCache.Allocate();
        sql.Append($"{GetQuotedColumnName(fieldDef.FieldName)} {fieldDefinition}");

        // Check for Composite PrimaryKey First
        if (modelDef.CompositePrimaryKeys.Any())
        {
            sql.Append(fieldDef.IsNullable ? " NULL" : " NOT NULL");
        }
        else
        {
            if (fieldDef.IsPrimaryKey)
            {
                sql.Append(" PRIMARY KEY");
                if (fieldDef.AutoIncrement)
                {
                    sql.Append(" ").Append(AutoIncrementDefinition);
                }
            }
            else
            {
                sql.Append(fieldDef.IsNullable ? " NULL" : " NOT NULL");
            }
        }

        if (fieldDef.IsUniqueConstraint)
        {
            sql.Append(" UNIQUE");
        }

        var defaultValue = this.GetDefaultValue(fieldDef);
        if (!string.IsNullOrEmpty(defaultValue))
        {
            if (fieldDef.ColumnType == typeof(bool))
            {
                sql.AppendFormat(this.DefaultValueFormat, defaultValue == "1" ? "TRUE" : "FALSE");
            }
            else
            {
                sql.AppendFormat(this.DefaultValueFormat, defaultValue);
            }
        }

        return StringBuilderCache.ReturnAndFree(sql);
    }

    /// <summary>
    /// Converts to createtablestatement.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <returns>System.String.</returns>
    public override string ToCreateTableStatement(Type tableType)
    {
        var sbColumns = StringBuilderCache.Allocate();
        var sbConstraints = StringBuilderCacheAlt.Allocate();

        var modelDef = GetModel(tableType);

        foreach (var fieldDef in this.CreateTableFieldsStrategy(modelDef))
        {
            if (fieldDef.ShouldSkipCreate())
            {
                continue;
            }

            var columnDefinition = this.GetColumnDefinition(fieldDef, modelDef);

            if (columnDefinition == null)
            {
                continue;
            }

            if (sbColumns.Length != 0)
            {
                sbColumns.Append(", \n  ");
            }

            sbColumns.Append(columnDefinition);

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
                $", \n\n  CONSTRAINT {this.GetQuotedName(fieldDef.ForeignKey.GetForeignKeyName(modelDef, refModelDef, this.NamingStrategy, fieldDef))} " +
                $"FOREIGN KEY ({this.GetQuotedColumnName(fieldDef.FieldName)}) " +
                $"REFERENCES {this.GetQuotedTableName(refModelDef)} ({this.GetQuotedColumnName(refModelDef.PrimaryKey)})");

            sbConstraints.Append(this.GetForeignKeyOnDeleteClause(fieldDef.ForeignKey));
            sbConstraints.Append(this.GetForeignKeyOnUpdateClause(fieldDef.ForeignKey));
        }

        var uniqueConstraints = this.GetUniqueConstraints(modelDef);
        if (uniqueConstraints != null)
        {
            sbConstraints.Append(",\n" + uniqueConstraints);
        }

        if (modelDef.CompositePrimaryKeys.Any())
        {
            sbConstraints.Append(",\n");

            var primaryKeyName = $"{this.NamingStrategy.GetTableName(modelDef)}_pkey";

            sbConstraints.AppendFormat(" CONSTRAINT {0} PRIMARY KEY (", primaryKeyName);

            sbConstraints.Append(
                modelDef.CompositePrimaryKeys.FirstOrDefault().FieldNames.Map(f => modelDef.GetQuotedName(f, this))
                    .Join(","));

            sbConstraints.Append(") ");
        }

        var sql = $"CREATE TABLE {this.GetQuotedTableName(modelDef)} " +
                  $"\n(\n  {StringBuilderCache.ReturnAndFree(sbColumns)}{StringBuilderCacheAlt.ReturnAndFree(sbConstraints)} \n); \n";

        return sql;
    }

    /// <summary>
    /// Gets the automatic identifier default value.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    public override string GetAutoIdDefaultValue(FieldDefinition fieldDef)
    {
        return fieldDef.FieldType == typeof(Guid)
                   ? AutoIdGuidFunction
                   : null;
    }

    /// <summary>
    /// The query that returns the query plan of the SQL
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="analyze">if set to <c>true</c> [analyze].</param>
    /// <returns>ExplainQuery.</returns>
    public override ExplainQuery ToExplainQuery(IDbConnection db, string sql, bool analyze) =>
        new() { Sql = (analyze ? "EXPLAIN ANALYZE " : "EXPLAIN ") + sql };

    /// <summary>
    /// Converts to returning statement.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="isDelete">if set to <c>true</c> [is delete].</param>
    /// <param name="returnFields">The fields to return, or null for all fields.</param>
    /// <returns>System.String.</returns>
    public override string ToReturningStatement(string sql, ModelDefinition modelDef, bool isDelete, ICollection<FieldDefinition> returnFields = null) =>
        sql.TrimEnd().TrimEnd(';') + " RETURNING " + this.GetReturningColumns(modelDef, returnFields);

    /// <summary>
    /// Determines whether [is full select statement] [the specified SQL].
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <returns><c>true</c> if [is full select statement] [the specified SQL]; otherwise, <c>false</c>.</returns>
    public override bool IsFullSelectStatement(string sql)
    {
        sql = sql?.TrimStart();
        if (string.IsNullOrEmpty(sql))
        {
            return false;
        }

        return sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) ||
               sql.StartsWith("WITH ", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Bulks the insert.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="db">The database.</param>
    /// <param name="objs">The objs.</param>
    /// <param name="config">The configuration.</param>
    public override void BulkInsert<T>(IDbConnection db, IEnumerable<T> objs, BulkInsertConfig config = null)
    {
        config ??= new();
        if (config.Mode == BulkInsertMode.Sql)
        {
            base.BulkInsert(db, objs, config);
            return;
        }

        var pgConn = (NpgsqlConnection)db.ToDbConnection();

        var modelDef = ModelDefinition<T>.Definition;

        var sb = StringBuilderCache.Allocate()
            .Append($"COPY {this.GetQuotedTableName(modelDef)} (");

        var fieldDefs = this.GetInsertFieldDefinitions(modelDef, insertFields: config.InsertFields);
        var i = 0;
        foreach (var fieldDef in fieldDefs)
        {
            if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
            {
                continue;
            }

            if (i++ > 0)
            {
                sb.Append(',');
            }

            sb.Append($"\"{this.GetQuotedColumnName(fieldDef)}\"");
        }

        sb.Append(") FROM STDIN (FORMAT BINARY)");

        var copyCmd = StringBuilderCache.ReturnAndFree(sb);
        using var writer = pgConn.BeginBinaryImport(copyCmd);

        foreach (var obj in objs)
        {
            writer.StartRow();
            foreach (var fieldDef in fieldDefs)
            {
                if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
                {
                    continue;
                }

                var value = fieldDef.AutoId
                    ? this.GetInsertDefaultValue(fieldDef)
                    : fieldDef.GetValue(obj);

                var converter = this.GetConverterBestMatch(fieldDef);
                if (converter == null)
                {
                    throw new NotSupportedException($"No converter found for {fieldDef.FieldType.Name}");
                }
                var dbValue = converter.ToDbValue(fieldDef.FieldType, value);
                if (dbValue is float f)
                {
                    dbValue = (double)f;
                }

                if (dbValue is null or DBNull)
                {
                    writer.WriteNull();
                }
                else
                {
                    try
                    {
                        var dbType = this.GetNpgsqlDbType(fieldDef);
                        if (dbType == NpgsqlDbType.Text && dbValue is not string && dbValue is not char)
                        {
                            dbValue = this.StringSerializer.SerializeToString(dbValue);
                        }

                        writer.Write(dbValue, dbType);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
        }

        writer.Complete();
    }

    protected override string ToBulkUpsertStatement(ModelDefinition modelDef, string stagingTable,
        List<FieldDefinition> insertFieldDefs, List<FieldDefinition> updateFieldDefs)
    {
        var conflictAction = updateFieldDefs.Count == 0
            ? "DO NOTHING"
            : "DO UPDATE SET " + updateFieldDefs.Map(x =>
                this.GetQuotedColumnName(x) + "=" + GetBulkUpsertValue(x.CustomUpdate, "EXCLUDED." + this.GetQuotedColumnName(x))).Join(", ");

        return $"{this.GetBulkUpsertInsertSql(modelDef, stagingTable, insertFieldDefs)} " +
               $"ON CONFLICT ({this.GetQuotedColumnName(modelDef.PrimaryKey)}) {conflictAction}";
    }

    // Rows are loaded with COPY
    protected override void BulkLoad<T>(IDbConnection db, string quotedTable, List<FieldDefinition> fieldDefs, IEnumerable<T> rows, BulkInsertConfig config)
    {
        if (config.Mode == BulkInsertMode.Sql)
        {
            base.BulkLoad(db, quotedTable, fieldDefs, rows, config);
            return;
        }

        var pgConn = (NpgsqlConnection)db.ToDbConnection();
        using var writer = pgConn.BeginBinaryImport(this.ToBinaryImportCommand(quotedTable, fieldDefs));
        foreach (var row in rows)
        {
            writer.StartRow();
            foreach (var fieldDef in fieldDefs)
            {
                var dbValue = this.ToBinaryImportDbValue(fieldDef, this.GetBulkLoadValue(fieldDef, row), out var dbType);
                if (dbValue == null)
                {
                    writer.WriteNull();
                }
                else
                {
                    writer.Write(dbValue, dbType);
                }
            }
        }
        writer.Complete();
    }

    private List<FieldDefinition> GetBinaryImportFieldDefinitions<T>(BulkInsertConfig config) =>
        this.GetInsertFieldDefinitions(ModelDefinition<T>.Definition, insertFields: config.InsertFields)
            .Where(x => !this.ShouldSkipInsert(x) || x.AutoId)
            .ToList();

    private string ToBinaryImportCommand<T>(List<FieldDefinition> fieldDefs) =>
        this.ToBinaryImportCommand(this.GetQuotedTableName(ModelDefinition<T>.Definition), fieldDefs);

    private string ToBinaryImportCommand(string quotedTable, List<FieldDefinition> fieldDefs) =>
        $"COPY {quotedTable} ({string.Join(",", fieldDefs.Select(x => this.GetQuotedColumnName(x)))}) FROM STDIN (FORMAT BINARY)";

    private object ToBinaryImportValue(FieldDefinition fieldDef, object obj, out NpgsqlDbType dbType)
    {
        var value = fieldDef.AutoId
            ? this.GetInsertDefaultValue(fieldDef)
            : fieldDef.GetValue(obj);
        return this.ToBinaryImportDbValue(fieldDef, value, out dbType);
    }

    private object ToBinaryImportDbValue(FieldDefinition fieldDef, object value, out NpgsqlDbType dbType)
    {
        dbType = default;

        var converter = this.GetConverterBestMatch(fieldDef)
            ?? throw new NotSupportedException($"No converter found for {fieldDef.FieldType.Name}");
        var dbValue = converter.ToDbValue(fieldDef.FieldType, value);
        if (dbValue is null or DBNull)
        {
            return null;
        }

        if (dbValue is float f)
        {
            dbValue = (double)f;
        }

        dbType = this.GetNpgsqlDbType(fieldDef);
        if (dbType == NpgsqlDbType.Text && dbValue is not string && dbValue is not char)
        {
            dbValue = this.StringSerializer.SerializeToString(dbValue);
        }

        return dbValue;
    }
    public NpgsqlDbType GetNpgsqlDbType(FieldDefinition fieldDef)
    {
        var converter = this.GetConverterBestMatch(fieldDef);

        var columnDef = fieldDef.CustomFieldDefinition ?? converter.ColumnDefinition;
        var dbType = converter.DbType;
        if (converter is ServiceStack.OrmLite.Converters.EnumConverter)
        {
            dbType = fieldDef.TreatAsType == typeof(int)
                ? DbType.Int32
                : fieldDef.TreatAsType == typeof(long)
                    ? DbType.Int64
                    : DbType.String;
        }
        return columnDef switch
        {
            "json" => NpgsqlDbType.Json,
            "jsonb" => NpgsqlDbType.Jsonb,
            "hstore" => NpgsqlDbType.Hstore,
            "text[]" => NpgsqlDbType.Array | NpgsqlDbType.Text,
            "short[]" => NpgsqlDbType.Array | NpgsqlDbType.Smallint,
            "integer[]" => NpgsqlDbType.Array | NpgsqlDbType.Integer,
            "bigint[]" => NpgsqlDbType.Array | NpgsqlDbType.Bigint,
            "real[]" => NpgsqlDbType.Array | NpgsqlDbType.Real,
            "double precision[]" => NpgsqlDbType.Array | NpgsqlDbType.Double,
            "double numeric[]" => NpgsqlDbType.Array | NpgsqlDbType.Numeric,
            "timestamp[]" => NpgsqlDbType.Array | NpgsqlDbType.Timestamp,
            "timestamp with time zone[]" => NpgsqlDbType.Array | NpgsqlDbType.TimestampTz,
            _ => dbType switch
            {
                DbType.Boolean => NpgsqlDbType.Boolean,
                DbType.SByte => NpgsqlDbType.Smallint,
                DbType.UInt16 => NpgsqlDbType.Smallint,
                DbType.Byte => NpgsqlDbType.Integer,
                DbType.Int16 => NpgsqlDbType.Integer,
                DbType.Int32 => NpgsqlDbType.Integer,
                DbType.UInt32 => NpgsqlDbType.Integer,
                DbType.Int64 => NpgsqlDbType.Bigint,
                DbType.UInt64 => NpgsqlDbType.Bigint,
                DbType.Single => NpgsqlDbType.Double,
                DbType.Double => NpgsqlDbType.Double,
                DbType.Decimal => NpgsqlDbType.Numeric,
                DbType.VarNumeric => NpgsqlDbType.Numeric,
                DbType.Currency => NpgsqlDbType.Money,
                DbType.Guid => NpgsqlDbType.Uuid,
                DbType.String => NpgsqlDbType.Text,
                DbType.AnsiString => NpgsqlDbType.Text,
                DbType.StringFixedLength => NpgsqlDbType.Text,
                DbType.AnsiStringFixedLength => NpgsqlDbType.Text,
                DbType.Xml => NpgsqlDbType.Text,
                DbType.Object => NpgsqlDbType.Text,
                DbType.Binary => NpgsqlDbType.Bytea,
                DbType.DateTime => NpgsqlDbType.TimestampTz,
                DbType.DateTimeOffset => NpgsqlDbType.TimestampTz,
                DbType.DateTime2 => NpgsqlDbType.Timestamp,
                DbType.Date => NpgsqlDbType.Date,
                DbType.Time => NpgsqlDbType.Time,
                _ => throw new AggregateException($"Unknown NpgsqlDbType for {fieldDef.FieldType} {fieldDef.Name}")
            }
        };
    }

    /// <summary>
    /// Shoulds the skip insert.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    override protected bool ShouldSkipInsert(FieldDefinition fieldDef) =>
        fieldDef.ShouldSkipInsert() || fieldDef.AutoId;

    /// <summary>
    /// Shoulds the return on insert.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected virtual bool ShouldReturnOnInsert(ModelDefinition modelDef, FieldDefinition fieldDef) =>
        fieldDef.ReturnOnInsert || fieldDef.IsPrimaryKey && fieldDef.AutoIncrement && HasInsertReturnValues(modelDef) || fieldDef.AutoId;

    /// <summary>
    /// Determines whether [has insert return values] [the specified model definition].
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns><c>true</c> if [has insert return values] [the specified model definition]; otherwise, <c>false</c>.</returns>
    public override bool HasInsertReturnValues(ModelDefinition modelDef) =>
        modelDef.FieldDefinitions.Any(x => x.ReturnOnInsert || x.AutoId && x.FieldType == typeof(Guid));

    /// <summary>
    /// Prepares the parameterized insert statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <param name="shouldInclude">The should include.</param>
    public override void PrepareParameterizedInsertStatement<T>(IDbCommand cmd, ICollection<string> insertFields = null,
                                                                Func<FieldDefinition,bool> shouldInclude=null)
    {
        var sbColumnNames = StringBuilderCache.Allocate();
        var sbColumnValues = StringBuilderCacheAlt.Allocate();
        var sbReturningColumns = StringBuilderCacheAlt.Allocate();
        var modelDef = OrmLiteUtils.GetModelDefinition(typeof(T));

        cmd.Parameters.Clear();

        var fieldDefs = GetInsertFieldDefinitions(modelDef, insertFields);
        foreach (var fieldDef in fieldDefs)
        {
            if (ShouldReturnOnInsert(modelDef, fieldDef))
            {
                sbReturningColumns.Append(sbReturningColumns.Length == 0 ? " RETURNING " : ",");
                var colName = fieldDef.IsRowVersion ? $"{RowVersionColumnName} AS {GetQuotedColumnName(fieldDef)}" : GetQuotedColumnName(fieldDef);
                sbReturningColumns.Append(colName);
            }

            if (ShouldSkipInsert(fieldDef) && !fieldDef.AutoId
                                           && shouldInclude?.Invoke(fieldDef) != true)
            {
                continue;
            }

            if (sbColumnNames.Length > 0)
            {
                sbColumnNames.Append(",");
            }

            if (sbColumnValues.Length > 0)
            {
                sbColumnValues.Append(",");
            }

            try
            {
                sbColumnNames.Append(GetQuotedColumnName(fieldDef.FieldName));

                sbColumnValues.Append(this.GetParam(SanitizeFieldNameForParamName(fieldDef.FieldName),fieldDef.CustomInsert));
                AddParameter(cmd, fieldDef);
            }
            catch (Exception ex)
            {
                Log.Error("ERROR in PrepareParameterizedInsertStatement(): " + ex.Message, ex);
                throw;
            }
        }

        foreach (var fieldDef in modelDef.AutoIdFields) // need to include any AutoId fields that weren't included
        {
            if (fieldDefs.Contains(fieldDef))
            {
                continue;
            }

            sbReturningColumns.Append(sbReturningColumns.Length == 0 ? " RETURNING " : ",");
            sbReturningColumns.Append(GetQuotedColumnName(fieldDef.FieldName));
        }

        var strReturning = StringBuilderCacheAlt.ReturnAndFree(sbReturningColumns);
        cmd.CommandText = sbColumnNames.Length > 0
                              ? $"INSERT INTO {GetQuotedTableName(modelDef)} ({StringBuilderCache.ReturnAndFree(sbColumnNames)}) " +
                                $"VALUES ({StringBuilderCacheAlt.ReturnAndFree(sbColumnValues)}){strReturning}"
                              : $"INSERT INTO {GetQuotedTableName(modelDef)} DEFAULT VALUES{strReturning}";
    }

    public override bool SupportsUpsert => true;

    public override string ToUpsertReturningStatement(string sql, ModelDefinition modelDef, ICollection<FieldDefinition> returnFields = null) =>
        this.ToReturningStatement(sql, modelDef, isDelete: false, returnFields);

    public override void PrepareParameterizedUpsertStatement<T>(IDbCommand cmd,
        ICollection<string> insertFields = null, ICollection<string> updateOnly = null)
    {
        PrepareUpsertFields<T>(cmd, insertFields, updateOnly,
            out var modelDef, out var insertFieldDefs, out var updateFieldDefs);

        var conflictTarget = GetQuotedColumnName(modelDef.PrimaryKey);
        var conflictAction = updateFieldDefs.Count == 0
            ? "DO NOTHING"
            : "DO UPDATE SET " + GetUpsertUpdateSql(updateFieldDefs);

        cmd.CommandText = $"{GetUpsertInsertSql(modelDef, insertFieldDefs)} " +
                          $"ON CONFLICT ({conflictTarget}) {conflictAction}";
    }

    //Convert xmin into an integer so it can be used in comparisons
    /// <summary>
    /// The row version field comparer
    /// </summary>
    public const string RowVersionFieldComparer = "int8in(xidout(xmin))";

    public const string RowVersionColumnName = "xmin";

    /// <summary>
    /// Gets the row version select column.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="tablePrefix">The table prefix.</param>
    /// <returns>SelectItem.</returns>
    public override SelectItem GetRowVersionSelectColumn(FieldDefinition field, string tablePrefix = null)
    {
        return new SelectItemColumn(this, RowVersionColumnName, field.FieldName, tablePrefix);
    }

    /// <summary>
    /// Gets the row version column.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="tablePrefix">The table prefix.</param>
    /// <returns>System.String.</returns>
    public override string GetRowVersionColumn(FieldDefinition field, string tablePrefix = null)
    {
        return RowVersionFieldComparer;
    }

    /// <summary>
    /// Appends the field condition.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="cmd">The command.</param>
    public override void AppendFieldCondition(StringBuilder sqlFilter, FieldDefinition fieldDef, IDbCommand cmd)
    {
        var columnName = fieldDef.IsRowVersion
                             ? RowVersionFieldComparer
                             : GetQuotedColumnName(fieldDef.FieldName);

        sqlFilter
            .Append(columnName)
            .Append("=")
            .Append(this.GetParam(SanitizeFieldNameForParamName(fieldDef.FieldName)));

        AddParameter(cmd, fieldDef);
    }

    /// <summary>
    /// Creates the connection.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="options">The options.</param>
    /// <returns>IDbConnection.</returns>
    public override IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options)
    {
        return new NpgsqlConnection(connectionString);
    }

    /// <summary>
    /// SQLs the expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public override SqlExpression<T> SqlExpression<T>()
    {
        return new PostgreSqlExpression<T>(this);
    }

    /// <summary>
    /// Creates the parameter.
    /// </summary>
    /// <returns>IDbDataParameter.</returns>
    public override IDbDataParameter CreateParam()
    {
        return new NpgsqlParameter();
    }

    /// <summary>
    /// Converts to tablenamesstatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    public override string ToTableNamesStatement(string schema)
    {
        var sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";

        var schemaName = schema != null
                             ? NamingStrategy.GetSchemaName(schema)
                             : "public";
        return sql + " AND table_schema = {0}".SqlFmt(this, schemaName);
    }

    /// <summary>
    /// Return table, row count SQL for listing all tables with their row counts
    /// </summary>
    /// <param name="live">If true returns live current row counts of each table (slower), otherwise returns cached row counts from RDBMS table stats</param>
    /// <param name="schema">The table schema if any</param>
    /// <returns>System.String.</returns>
    public override string ToTableNamesWithRowCountsStatement(bool live, string schema)
    {
        var schemaName = schema != null
                             ? NamingStrategy.GetSchemaName(schema)
                             : "public";
        return live
                   ? null
                   : "SELECT relname, reltuples FROM pg_class JOIN pg_catalog.pg_namespace n ON n.oid = pg_class.relnamespace WHERE relkind = 'r' AND nspname = {0}".SqlFmt(this, schemaName);
    }

    public override List<string> GetTableIndexNames(IDbConnection db, TableRef tableRef)
    {
        var schema = this.GetSchemaName(tableRef);
        var sql = "SELECT indexname FROM pg_indexes WHERE lower(tablename) = {0}".SqlFmt(this, this.GetTableNameOnly(tableRef).ToLower())
            + (schema != null
                ? " AND lower(schemaname) = {0}".SqlFmt(this, schema.ToLower())
                : " AND schemaname = current_schema()");
        return db.Column<string>(sql);
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
        var sql = DoesTableExistSql(dbCmd, tableRef);
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
    public async override Task<bool> DoesTableExistAsync(IDbCommand dbCmd, TableRef tableRef, CancellationToken token=default)
    {
        var sql = DoesTableExistSql(dbCmd, tableRef);
        var result = await dbCmd.ExecLongScalarAsync(sql, token);
        return result > 0;
    }

    /// <summary>
    /// Doeses the table exist SQL.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    private string DoesTableExistSql(IDbCommand dbCmd, TableRef tableRef)
    {
        var tableName = GetTableNameOnly(tableRef);
        var schema = GetSchemaName(tableRef);

        var sql = !Normalize || ReservedWords.Contains(tableName)
            ? "SELECT COUNT(*) FROM pg_class WHERE relname = {0} AND relkind = 'r'".SqlFmt(this, tableName)
            : "SELECT COUNT(*) FROM pg_class WHERE lower(relname) = {0} AND relkind = 'r'".SqlFmt(this, tableName.ToLower());

        var conn = dbCmd.Connection;
        if (conn != null)
        {
            var builder = new NpgsqlConnectionStringBuilder(conn.ConnectionString);
            if (schema == null)
            {
                schema = builder.SearchPath;
            }

            if (schema == null)
            {
                schema = "public";
            }

            // If a search path (schema) is specified, and there is only one, then assume the CREATE TABLE directive should apply to that schema.
            if (!string.IsNullOrEmpty(schema) && !schema.Contains(","))
            {
                sql = !Normalize || ReservedWords.Contains(schema)
                          ? "SELECT COUNT(*) FROM pg_class JOIN pg_catalog.pg_namespace n ON n.oid = pg_class.relnamespace WHERE relname = {0} AND relkind = 'r' AND nspname = {1}"
                              .SqlFmt(this, tableName, schema)
                          : "SELECT COUNT(*) FROM pg_class JOIN pg_catalog.pg_namespace n ON n.oid = pg_class.relnamespace WHERE lower(relname) = {0} AND relkind = 'r' AND lower(nspname) = {1}"
                              .SqlFmt(this, tableName.ToLower(), schema.ToLower());
            }
        }

        return sql;
    }

    public override List<string> GetSchemas(IDbCommand dbCmd)
    {
        var sql = "SELECT DISTINCT TABLE_SCHEMA FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA NOT IN ('information_schema', 'pg_catalog')";
        return dbCmd.SqlColumn<string>(sql);
    }

    public override Dictionary<string, List<string>> GetSchemaTables(IDbCommand dbCmd)
    {
        var sql = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA NOT IN ('information_schema', 'pg_catalog')";
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
        dbCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = '{NamingStrategy.GetSchemaName(schemaName).SqlParam()}');";
        var query = dbCmd.ExecuteScalar();
        return query as bool? ?? false;
    }

    /// <summary>
    /// Does schema exist as an asynchronous operation.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="schemaName">Name of the schema.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
    public async override Task<bool> DoesSchemaExistAsync(IDbCommand dbCmd, string schemaName, CancellationToken token = default)
    {
        dbCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = '{NamingStrategy.GetSchemaName(schemaName).SqlParam()}');";
        var query = await dbCmd.ScalarAsync();
        return query as bool? ?? false;
    }

    /// <summary>
    /// Converts to createschemastatement.
    /// </summary>
    /// <param name="schemaName">Name of the schema.</param>
    /// <returns>System.String.</returns>
    public override string ToCreateSchemaStatement(string schemaName)
    {
        var sql = $"CREATE SCHEMA {GetQuotedName(NamingStrategy.GetSchemaName(schemaName))}";
        return sql;
    }

    /// <summary>
    /// Doeses the column exist.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool DoesColumnExist(IDbConnection db, string columnName, TableRef tableRef)
    {
        var tableName = GetTableNameOnly(tableRef);
        var schema = GetSchemaName(tableRef);
        var sql = DoesColumnExistSql(columnName, tableName, ref schema);
        var result = db.SqlScalar<long>(sql, new { tableName, columnName, schema });
        return result > 0;
    }

    /// <summary>
    /// Does column exist as an asynchronous operation.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
    public async override Task<bool> DoesColumnExistAsync(IDbConnection db, string columnName, TableRef tableRef,
                                                          CancellationToken token = default)
    {
        var tableName = GetTableNameOnly(tableRef);
        var schema = GetSchemaName(tableRef);
        var sql = DoesColumnExistSql(columnName, tableName, ref schema);
        var result = await db.SqlScalarAsync<long>(sql, new { tableName, columnName, schema }, token);
        return result > 0;
    }

    /// <summary>
    /// Doeses the column exist SQL.
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    private string DoesColumnExistSql(string columnName, string tableName, ref string schema)
    {
        var sql = !Normalize || ReservedWords.Contains(tableName)
                      ? "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName".SqlFmt(this, tableName)
                      : "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE lower(TABLE_NAME) = @tableName".SqlFmt(this,
                          tableName.ToLower());

        sql += !Normalize || ReservedWords.Contains(columnName)
            ? " AND COLUMN_NAME = @columnName".SqlFmt(this, columnName)
            : " AND lower(COLUMN_NAME) = @columnName".SqlFmt(this, columnName.ToLower());

        if (schema != null)
        {
            sql += !Normalize || ReservedWords.Contains(schema)
                       ? " AND TABLE_SCHEMA = @schema"
                       : " AND lower(TABLE_SCHEMA) = @schema";

            if (this.Normalize)
            {
                schema = schema.ToLower();
            }
        }

        return sql;
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
        var tableName = this.UnquotedTable(tableRef);
        var sql =
            "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName"
                .SqlFmt(this, tableName, columnName);

        var schema = this.GetSchemaName(tableRef);
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
        var tableName = this.UnquotedTable(tableRef);
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
    /// Converts to executeprocedurestatement.
    /// </summary>
    /// <param name="objWithProperties">The object with properties.</param>
    /// <returns>System.String.</returns>
    public override string ToExecuteProcedureStatement(object objWithProperties)
    {
        var sbColumnValues = StringBuilderCache.Allocate();

        var tableType = objWithProperties.GetType();
        var modelDef = GetModel(tableType);

        foreach (var fieldDef in modelDef.FieldDefinitions)
        {
            if (sbColumnValues.Length > 0)
            {
                sbColumnValues.Append(",");
            }

            sbColumnValues.Append(fieldDef.GetQuotedValue(objWithProperties));
        }

        var colValues = StringBuilderCache.ReturnAndFree(sbColumnValues);
        var sql =
            $"{GetQuotedTableName(modelDef)} {(colValues.Length > 0 ? "(" : "")}{colValues}{(colValues.Length > 0 ? ")" : "")};";

        return sql;
    }

    /// <summary>
    /// Converts to altercolumnstatement.
    /// </summary>
    /// <param name="modelType">Type of the model.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    public override string ToAlterColumnStatement(TableRef tableRef, FieldDefinition fieldDef)
    {
        var columnDefinition = GetColumnDefinition(fieldDef);
        var modelName = QuoteTable(tableRef);

        var parts = columnDefinition.SplitOnFirst(' ');
        var columnName = parts[0];
        var columnType = parts[1];

        var notNull = columnDefinition.Contains("NOT NULL");

        var nullLiteral = notNull ? " NOT NULL" : " NULL";
        columnType = columnType.Replace(nullLiteral, "");

        var nullSql = notNull
                          ? "SET NOT NULL"
                          : "DROP NOT NULL";

        var sql = $"ALTER TABLE {modelName}\n"
                  + $"  ALTER COLUMN {columnName} TYPE {columnType},\n"
                  + $"  ALTER COLUMN {columnName} {nullSql}";

        return sql;
    }

    /// <summary>
    /// Shoulds the quote.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool ShouldQuote(string name) => !string.IsNullOrEmpty(name) &&
                                                     (Normalize || ReservedWords.Contains(name) || name.IndexOf(' ') >= 0 || name.IndexOf('.') >= 0);

    /// <summary>
    /// Gets the name of the quoted.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public override string GetQuotedName(string name)
    {
        if (name == null)
        {
            return null;
        }

        return name.IndexOf('.') >= 0
                   ? string.Join(".", name.Split('.').Map(part => base.GetQuotedName(part)))
                   : base.GetQuotedName(name);
    }

    /// <summary>
    /// Gets the name of the quoted table.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public override string GetQuotedTableName(ModelDefinition modelDef)
    {
        if (modelDef == null)
        {
            return null;
        }

        return !modelDef.IsInSchema
            ? base.GetQuotedTableName(modelDef)
            : $"{GetQuotedName(NamingStrategy.GetSchemaName(modelDef.Schema))}.{GetQuotedName(NamingStrategy.GetTableName(modelDef))}";
    }

    /// <summary>
    /// Gets the last insert identifier SQL suffix.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>System.String.</returns>
    /// <exception cref="System.NotImplementedException">Returning last inserted identity is not implemented on this DB Provider.</exception>
    public override string GetLastInsertIdSqlSuffix<T>()
    {
        if (this.SelectIdentitySql == null)
        {
            throw new NotImplementedException("Returning last inserted identity is not implemented on this DB Provider.");
        }

        if (UseReturningForLastInsertId)
        {
            var modelDef = GetModel(typeof(T));
            var pkName = GetQuotedColumnName(modelDef.PrimaryKey);
            return " RETURNING " + pkName;
        }

        return "; " + SelectIdentitySql;
    }

    /// <summary>
    /// Gets the types map.
    /// </summary>
    /// <value>The types map.</value>
    public Dictionary<Type, NpgsqlDbType> TypesMap { get; } = new()
    {
        [typeof(bool)] = NpgsqlDbType.Boolean,
        [typeof(short)] = NpgsqlDbType.Smallint,
        [typeof(int)] = NpgsqlDbType.Integer,
        [typeof(long)] = NpgsqlDbType.Bigint,
        [typeof(float)] = NpgsqlDbType.Real,
        [typeof(double)] = NpgsqlDbType.Double,
        [typeof(decimal)] = NpgsqlDbType.Numeric,
        [typeof(string)] = NpgsqlDbType.Text,
        [typeof(char[])] = NpgsqlDbType.Varchar,
        [typeof(char)] = NpgsqlDbType.Char,
        [typeof(NpgsqlPoint)] = NpgsqlDbType.Point,
        [typeof(NpgsqlLSeg)] = NpgsqlDbType.LSeg,
        [typeof(NpgsqlPath)] = NpgsqlDbType.Path,
        [typeof(NpgsqlPolygon)] = NpgsqlDbType.Polygon,
        [typeof(NpgsqlLine)] = NpgsqlDbType.Line,
        [typeof(NpgsqlCircle)] = NpgsqlDbType.Circle,
        [typeof(NpgsqlBox)] = NpgsqlDbType.Box,
        [typeof(BitArray)] = NpgsqlDbType.Varbit,
        [typeof(IDictionary<string, string>)] = NpgsqlDbType.Hstore,
        [typeof(Guid)] = NpgsqlDbType.Uuid,
        [typeof(ValueTuple<IPAddress, int>)] = NpgsqlDbType.Cidr,
        [typeof(ValueTuple<IPAddress, int>)] = NpgsqlDbType.Inet,
        [typeof(IPAddress)] = NpgsqlDbType.Inet,
        [typeof(PhysicalAddress)] = NpgsqlDbType.MacAddr,
        [typeof(NpgsqlTsQuery)] = NpgsqlDbType.TsQuery,
        [typeof(NpgsqlTsVector)] = NpgsqlDbType.TsVector,
#if NET6_0
                                                                           [typeof(DateOnly)] = NpgsqlDbType.Dat4e,
                                                                           [typeof(TimeOnly)] = NpgsqlDbType.Time,  
#endif
                                                                           [typeof(DateTime)] = NpgsqlDbType.Timestamp,
                                                                           [typeof(DateTimeOffset)] = NpgsqlDbType.TimestampTz,
                                                                           [typeof(TimeSpan)] = NpgsqlDbType.Time,
                                                                           [typeof(byte[])] = NpgsqlDbType.Bytea,
                                                                           [typeof(uint)] = NpgsqlDbType.Oid,
                                                                           [typeof(uint[])] = NpgsqlDbType.Oidvector
    };

    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>NpgsqlDbType.</returns>
    public NpgsqlDbType GetDbType<T>() => GetDbType(typeof(T));
    /// <summary>
    /// Gets the type of the database.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>NpgsqlDbType.</returns>
    /// <exception cref="System.NotSupportedException">Type '{type.Name}' not found in 'TypesMap'</exception>
    public NpgsqlDbType GetDbType(Type type)
    {
        if (this.TypesMap.TryGetValue(type, out var paramType))
        {
            return paramType;
        }

        var genericEnum = type.GetTypeWithGenericTypeDefinitionOf(typeof(IEnumerable<>));
        if (genericEnum != null)
        {
            return this.GetDbType(genericEnum.GenericTypeArguments[0]) | NpgsqlDbType.Array;
        }

        throw new NotSupportedException($"Type '{type.Name}' not found in 'TypesMap'");
    }

    /// <summary>
    /// The native types
    /// </summary>
    public Dictionary<string, NpgsqlDbType> NativeTypes = new() {
                                                                        { "json", NpgsqlDbType.Json },
                                                                        { "jsonb", NpgsqlDbType.Jsonb },
                                                                        { "hstore", NpgsqlDbType.Hstore },
                                                                        { "text[]", NpgsqlDbType.Array | NpgsqlDbType.Text },
                                                                        { "short[]", NpgsqlDbType.Array | NpgsqlDbType.Smallint },
                                                                        { "int[]", NpgsqlDbType.Array | NpgsqlDbType.Integer },
                                                                        { "integer[]", NpgsqlDbType.Array | NpgsqlDbType.Integer },
                                                                        { "bigint[]", NpgsqlDbType.Array | NpgsqlDbType.Bigint },
                                                                        { "real[]", NpgsqlDbType.Array | NpgsqlDbType.Real },
                                                                        { "double precision[]", NpgsqlDbType.Array | NpgsqlDbType.Double },
                                                                        { "numeric[]", NpgsqlDbType.Array | NpgsqlDbType.Numeric },
                                                                        { "timestamp[]", NpgsqlDbType.Array | NpgsqlDbType.Timestamp },
                                                                        { "timestamp with time zone[]", NpgsqlDbType.Array | NpgsqlDbType.TimestampTz },
                                                                        { "bool[]", NpgsqlDbType.Array | NpgsqlDbType.Boolean },
                                                                        { "boolean[]", NpgsqlDbType.Array | NpgsqlDbType.Boolean }
    };

    // Vectors use the vector type of the pgvector extension: CREATE EXTENSION vector
    public override string GetVectorColumnDefinition(int dimensions) => $"vector({dimensions})";

    public override string ToVectorParam(string param, int dimensions) => param + "::vector";

    public override string ToVectorDistance(VectorDistance distance, string vector, string other) =>
        $"({vector} {VectorOperator(distance)} {other})";

    private static string VectorOperator(VectorDistance distance) => distance switch
    {
        VectorDistance.Cosine => "<=>",
        VectorDistance.L2 => "<->",
        _ => "<#>",
    };

    // Npgsql can't read the vector type without a plugin, so it's read as text
    protected override string GetVectorSelectExpression(string quotedColumn) => quotedColumn + "::text";

    protected override string ToCreateVectorIndexStatement(ModelDefinition modelDef, FieldDefinition fieldDef, string indexName)
    {
        var operatorClass = fieldDef.VectorDistance switch
        {
            VectorDistance.Cosine => "vector_cosine_ops",
            VectorDistance.L2 => "vector_l2_ops",
            _ => "vector_ip_ops",
        };
        return $"CREATE INDEX {indexName} ON {this.GetQuotedTableName(modelDef)} USING hnsw ({this.GetQuotedColumnName(fieldDef)} {operatorClass}); \n";
    }

    /// <summary>
    /// Sets the parameter.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="p">The p.</param>
    public override void SetParameter(FieldDefinition fieldDef, IDbDataParameter p)
    {
        if (fieldDef.VectorDimensions != null)
        {
            // Sent as text without a type, which PostgreSQL converts to the type of its column
            p.ParameterName = this.GetParam(this.SanitizeFieldNameForParamName(fieldDef.FieldName));
            ((NpgsqlParameter)p).NpgsqlDbType = NpgsqlDbType.Unknown;
        }
        else if (fieldDef.CustomFieldDefinition != null &&
            NativeTypes.TryGetValue(fieldDef.CustomFieldDefinition, out var npgsqlDbType))
        {
            p.ParameterName = this.GetParam(SanitizeFieldNameForParamName(fieldDef.FieldName));
            ((NpgsqlParameter) p).NpgsqlDbType = npgsqlDbType;
        }
        else
        {
            base.SetParameter(fieldDef, p);
        }
    }

    /// <summary>
    /// Uses the raw value.
    /// </summary>
    /// <param name="columnDef">The column definition.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public virtual bool UseRawValue(string columnDef) => columnDef?.EndsWith("[]") == true;

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="obj">The object.</param>
    /// <returns>System.Object.</returns>
    override protected object GetValue(FieldDefinition fieldDef, object obj)
    {
        if (fieldDef.CustomFieldDefinition != null && NativeTypes.ContainsKey(fieldDef.CustomFieldDefinition)
                                                   && UseRawValue(fieldDef.CustomFieldDefinition))
        {
            return fieldDef.GetValue(obj);
        }

        return base.GetValue(fieldDef, obj);
    }

    /// <summary>
    /// Prepares the stored procedure statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    /// <param name="obj">The object.</param>
    public override void PrepareStoredProcedureStatement<T>(IDbCommand cmd, T obj)
    {
        var tableType = obj.GetType();
        var modelDef = GetModel(tableType);

        cmd.CommandText = GetQuotedTableName(modelDef);
        cmd.CommandType = CommandType.StoredProcedure;

        foreach (var fieldDef in modelDef.FieldDefinitions)
        {
            var p = cmd.CreateParameter();
            SetParameter(fieldDef, p);
            cmd.Parameters.Add(p);
        }

        SetParameterValues<T>(cmd, obj);
    }
    public override string ToChangeColumnNameStatement(TableRef tableRef, FieldDefinition fieldDef, string oldColumn)
    {
        //var column = GetColumnDefinition(fieldDef);
        var columnType = GetColumnTypeDefinition(fieldDef.ColumnType, fieldDef.FieldLength, fieldDef.Scale);
        var newColumnName = NamingStrategy.GetColumnName(fieldDef.FieldName);

        var sql = $"ALTER TABLE {QuoteTable(tableRef)} " +
                  $"ALTER COLUMN {GetQuotedColumnName(oldColumn)} TYPE {columnType}";
        sql += newColumnName != oldColumn
                   ? $", RENAME COLUMN {GetQuotedColumnName(oldColumn)} TO {GetQuotedColumnName(newColumnName)};"
                   : ";";
        return sql;
    }

    public override string ToResetSequenceStatement(Type tableType, string columnName, int value)
    {
        base.ToResetSequenceStatement(tableType, columnName, value);
        var modelDef = GetModel(tableType);
        var fieldDef = modelDef.GetFieldDefinition(columnName);
        // Table needs to be quoted but not column
        var useTable = GetQuotedTableName(modelDef);
        var useColumn = fieldDef != null ? NamingStrategy.GetColumnName(fieldDef.FieldName) : columnName;

        return $"SELECT setval(pg_get_serial_sequence('{useTable.SqlParam()}', '{useColumn.SqlParam()}'), {value}, false);";
    }

    /// <summary>
    /// SQLs the conflict.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="conflictResolution">The conflict resolution.</param>
    /// <returns>System.String.</returns>
    public override string SqlConflict(string sql, string conflictResolution)
    {
        //https://www.postgresql.org/docs/current/static/sql-insert.html
        return sql + " ON CONFLICT " + (conflictResolution == ConflictResolution.Ignore
                                            ? " DO NOTHING"
                                            : conflictResolution);
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
    public override string SqlCurrency(string fieldOrValue, string currencySymbol) => currencySymbol == "$"
        ? fieldOrValue + "::text::money::text"
        : "replace(" + fieldOrValue + "::text::money::text,'$'," + GetQuotedValue(currencySymbol) + ")";

    /// <summary>
    /// SQLs the cast.
    /// </summary>
    /// <param name="fieldOrValue">The field or value.</param>
    /// <param name="castAs">The cast as.</param>
    /// <returns>System.String.</returns>
    public override string SqlCast(object fieldOrValue, string castAs) =>
        $"({fieldOrValue})::{castAs}";

    /// <summary>
    /// Gets the SQL random.
    /// </summary>
    /// <value>The SQL random.</value>
    public override string SqlRandom => "RANDOM()";

    /// <summary>
    /// Unwraps the specified database.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <returns>NpgsqlConnection.</returns>
    protected DbConnection Unwrap(IDbConnection db) => (DbConnection)db.ToDbConnection();

    /// <summary>
    /// SQLSTATEs where PostgreSQL confirmed the statement wasn't applied: serialization failures, deadlocks, locks
    /// that weren't available and connections that couldn't be made
    /// </summary>
    public static HashSet<string> NotAppliedErrors { get; } = [
        "40001", "40P01", "55P03", "53300", "53400", "57P03", "08001", "08004",
    ];

    /// <summary>
    /// SQLSTATEs where the connection was lost, so the statement may have been applied
    /// </summary>
    public static HashSet<string> ConnectionLostErrors { get; } = [
        "08000", "08003", "08006", "57P01", "57P02",
    ];

    public override TransientError GetTransientError(Exception ex)
    {
        if (ex is PostgresException pgEx)
        {
            if (NotAppliedErrors.Contains(pgEx.SqlState))
            {
                return TransientError.NotApplied;
            }

            if (ConnectionLostErrors.Contains(pgEx.SqlState))
            {
                return TransientError.MaybeApplied;
            }

            return TransientError.None;
        }
        return base.GetTransientError(ex);
    }

    /// <summary>
    /// Unwraps the specified command.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <returns>NpgsqlCommand.</returns>
    protected DbCommand Unwrap(IDbCommand cmd) => (DbCommand)cmd.ToDbCommand();

    /// <summary>
    /// Unwraps the specified reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>NpgsqlDataReader.</returns>
    protected DbDataReader Unwrap(IDataReader reader) => (DbDataReader)reader;

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
        return Unwrap(db).OpenAsync(token);
    }

    /// <summary>
    /// Executes the reader asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;IDataReader&gt;.</returns>
    public override Task<IDataReader> ExecuteReaderAsync(IDbCommand cmd, CancellationToken token = default)
    {
        return Unwrap(cmd).ExecuteReaderAsync(token).Then(x => (IDataReader)x);
    }

    /// <summary>
    /// Executes the non query asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    public override Task<int> ExecuteNonQueryAsync(IDbCommand cmd, CancellationToken token = default)
    {
        return Unwrap(cmd).ExecuteNonQueryAsync(token);
    }

    /// <summary>
    /// Executes the scalar asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Object&gt;.</returns>
    public override Task<object> ExecuteScalarAsync(IDbCommand cmd, CancellationToken token = default)
    {
        return Unwrap(cmd).ExecuteScalarAsync(token);
    }

    /// <summary>
    /// Reads the asynchronous.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public override Task<bool> ReadAsync(IDataReader reader, CancellationToken token = default)
    {
        return Unwrap(reader).ReadAsync(token);
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
            while (await ReadAsync(reader, token).ConfigureAwait(false))
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
    public async override Task<Return> ReaderEach<Return>(IDataReader reader, Action fn, Return source, CancellationToken token = default)
    {
        try
        {
            while (await ReadAsync(reader, token).ConfigureAwait(false))
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
        sb.AppendFormat("{0}", tableName);

        sb.Append(";");

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

    public override string GetAddCompositePrimaryKey(string database, ModelDefinition modelDef, string fieldNameA, string fieldNameB)
    {
        var sb = StringBuilderCache.Allocate();

        var tableName = this.GetQuotedTableName(modelDef);

        sb.AppendFormat(
            "ALTER TABLE {0} ADD PRIMARY KEY ({1},{2})",
            tableName,
            this.GetQuotedColumnName(fieldNameA),
            this.GetQuotedColumnName(fieldNameB));

        return StringBuilderCache.ReturnAndFree(sb);
    }

    public override string GetPrimaryKeyName(ModelDefinition modelDef)
    {
        return $"{this.NamingStrategy.GetTableName(modelDef)}_pkey";
    }

    /// <summary>
    /// Gets the drop primary key constraint.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public override string GetDropPrimaryKeyConstraint(string database, ModelDefinition modelDef, string name)
    {
        var sb = StringBuilderCache.Allocate();

        var tableName = this.GetQuotedTableName(modelDef);

        sb.AppendFormat("ALTER TABLE {1} DROP constraint {0}", this.GetQuotedName(name), tableName);

        return StringBuilderCache.ReturnAndFree(sb);
    }

    public override string GetDropPrimaryKeyConstraint(string database, ModelDefinition modelDef, string name, string fieldNameA, string fieldNameB)
    {
        var sb = StringBuilderCache.Allocate();

        var tableName = this.GetQuotedTableName(modelDef);

        sb.AppendFormat("ALTER TABLE {1} DROP constraint {0}", this.GetQuotedName(name), tableName);

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Gets the UTC date function.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string GetUtcDateFunction()
    {
        return "(now() at time zone 'utc')";
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
            ? $"EXTRACT(Minute from ({date2}::timestamp - {date1}::timestamp))"
            : $"EXTRACT(Day from ({date2}::timestamp - {date1}::timestamp))";
    }

    /// <summary>
    /// Gets the SQL ISNULL Function
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="alternateValue">The alternate Value.</param>
    /// <returns>The <see cref="string" />.</returns>
    public override string IsNullFunction(string expression, object alternateValue)
    {
        return $"COALESCE(({expression}), {alternateValue})";
    }

    /// <summary>
    /// Converts the flag.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>System.String.</returns>
    public override string ConvertFlag(string expression)
    {
        return $"CAST(CAST(SIGN({expression}) AS char(1)) AS boolean)";
    }

    /// <summary>
    /// Databases the fragmentation information.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>System.String.</returns>
    public override string DatabaseFragmentationInfo(string database)
    {
        var sb = new StringBuilder();

        // Code From https://github.com/ioguix/pgsql-bloat-estimation/tree/master/table
        sb.Append(@"SELECT current_database(), schemaname, tblname, bs*tblpages AS real_size,
  (tblpages-est_tblpages)*bs AS extra_size,
  CASE WHEN tblpages - est_tblpages > 0
    THEN 100 * (tblpages - est_tblpages)/tblpages::float
    ELSE 0
  END AS extra_ratio, fillfactor,
  CASE WHEN tblpages - est_tblpages_ff > 0
    THEN (tblpages-est_tblpages_ff)*bs
    ELSE 0
  END AS bloat_size,
  CASE WHEN tblpages - est_tblpages_ff > 0
    THEN 100 * (tblpages - est_tblpages_ff)/tblpages::float
    ELSE 0
  END AS bloat_ratio, is_na
  -- , tpl_hdr_size, tpl_data_size, (pst).free_percent + (pst).dead_tuple_percent AS real_frag -- (DEBUG INFO)
FROM (
  SELECT ceil( reltuples / ( (bs-page_hdr)/tpl_size ) ) + ceil( toasttuples / 4 ) AS est_tblpages,
    ceil( reltuples / ( (bs-page_hdr)*fillfactor/(tpl_size*100) ) ) + ceil( toasttuples / 4 ) AS est_tblpages_ff,
    tblpages, fillfactor, bs, tblid, schemaname, tblname, heappages, toastpages, is_na
    -- , tpl_hdr_size, tpl_data_size, pgstattuple(tblid) AS pst -- (DEBUG INFO)
  FROM (
    SELECT
      ( 4 + tpl_hdr_size + tpl_data_size + (2*ma)
        - CASE WHEN tpl_hdr_size%ma = 0 THEN ma ELSE tpl_hdr_size%ma END
        - CASE WHEN ceil(tpl_data_size)::int%ma = 0 THEN ma ELSE ceil(tpl_data_size)::int%ma END
      ) AS tpl_size, bs - page_hdr AS size_per_block, (heappages + toastpages) AS tblpages, heappages,
      toastpages, reltuples, toasttuples, bs, page_hdr, tblid, schemaname, tblname, fillfactor, is_na
      -- , tpl_hdr_size, tpl_data_size
    FROM (
      SELECT
        tbl.oid AS tblid, ns.nspname AS schemaname, tbl.relname AS tblname, tbl.reltuples,
        tbl.relpages AS heappages, coalesce(toast.relpages, 0) AS toastpages,
        coalesce(toast.reltuples, 0) AS toasttuples,
        coalesce(substring(
          array_to_string(tbl.reloptions, ' ')
          FROM 'fillfactor=([0-9]+)')::smallint, 100) AS fillfactor,
        current_setting('block_size')::numeric AS bs,
        CASE WHEN version()~'mingw32' OR version()~'64-bit|x86_64|ppc64|ia64|amd64' THEN 8 ELSE 4 END AS ma,
        24 AS page_hdr,
        23 + CASE WHEN MAX(coalesce(s.null_frac,0)) > 0 THEN ( 7 + count(s.attname) ) / 8 ELSE 0::int END
           + CASE WHEN bool_or(att.attname = 'oid' and att.attnum < 0) THEN 4 ELSE 0 END AS tpl_hdr_size,
        sum( (1-coalesce(s.null_frac, 0)) * coalesce(s.avg_width, 0) ) AS tpl_data_size,
        bool_or(att.atttypid = 'pg_catalog.name'::regtype)
          OR sum(CASE WHEN att.attnum > 0 THEN 1 ELSE 0 END) <> count(s.attname) AS is_na
      FROM pg_attribute AS att
        JOIN pg_class AS tbl ON att.attrelid = tbl.oid
        JOIN pg_namespace AS ns ON ns.oid = tbl.relnamespace
        LEFT JOIN pg_stats AS s ON s.schemaname=ns.nspname
          AND s.tablename = tbl.relname AND s.inherited=false AND s.attname=att.attname
        LEFT JOIN pg_class AS toast ON tbl.reltoastrelid = toast.oid
      WHERE NOT att.attisdropped
        AND tbl.relkind in ('r','m')
      GROUP BY 1,2,3,4,5,6,7,8,9,10
      ORDER BY 2,3
    ) AS s
  ) AS s2
) AS s3
-- WHERE NOT is_na
--   AND tblpages*((pst).free_percent + (pst).dead_tuple_percent)::float4/100 >= 1
ORDER BY schemaname, tblname;");

        return sb.ToString();
    }

    /// <summary>
    /// Databases the size.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>System.String.</returns>
    public override string DatabaseSize(string database)
    {
        return "select pg_database_size(current_database()) / 1024 / 1024";
    }

    /// <summary>
    /// SQLs the version.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string SQLVersion()
    {
        return "select version()";
    }

    /// <summary>
    /// SQLs the name of the server.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string SQLServerName()
    {
        return "PostgreSQL";
    }

    /// <summary>
    /// Shrinks the database.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>System.String.</returns>
    public override string ShrinkDatabase(string database)
    {
        return "VACUUM";
    }

    /// <summary>
    /// Res the index database.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="objectQualifier">The object qualifier.</param>
    /// <returns>System.String.</returns>
    public override string ReIndexDatabase(string database, string objectQualifier)
    {
        return $"REINDEX DATABASE {database};";
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
}
