namespace ServiceStack.OrmLite.MySql
{
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
    using ServiceStack.Script;
    using ServiceStack.Text;

    public abstract class MySqlDialectProviderBase<TDialect> : OrmLiteDialectProviderBase<TDialect> where TDialect : IOrmLiteDialectProvider
    {

        private const string TextColumnDefinition = "TEXT";

        public MySqlDialectProviderBase()
        {
            base.AutoIncrementDefinition = "AUTO_INCREMENT";
            base.DefaultValueFormat = " DEFAULT {0}";
            base.SelectIdentitySql = "SELECT LAST_INSERT_ID()";

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

            this.Variables = new Dictionary<string, string>
            {
                { OrmLiteVariables.SystemUtc, "UTC_TIMESTAMP" },
                { OrmLiteVariables.MaxText, "LONGTEXT" },
                { OrmLiteVariables.MaxTextUnicode, "LONGTEXT" },
                { OrmLiteVariables.True, SqlBool(true) },
                { OrmLiteVariables.False, SqlBool(false) },
            };
        }

        public static string RowVersionTriggerFormat = "{0}RowVersionUpdateTrigger";

        public static HashSet<string> ReservedWords = new HashSet<string>(new[]
        {
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
          "ZEROFILL",
        }, StringComparer.OrdinalIgnoreCase);

        public override string GetLoadChildrenSubSelect<From>(SqlExpression<From> expr)
        {
            return $"SELECT * FROM ({base.GetLoadChildrenSubSelect(expr)}) AS SubQuery";
        }
        public override string ToPostDropTableStatement(ModelDefinition modelDef)
        {
            if (modelDef.RowVersion != null)
            {
                var triggerName = RowVersionTriggerFormat.Fmt(GetTableName(modelDef));
                return "DROP TRIGGER IF EXISTS {0}".Fmt(GetQuotedName(triggerName));
            }
            return null;
        }

        public override string ToPostCreateTableStatement(ModelDefinition modelDef)
        {
            if (modelDef.RowVersion != null)
            {
                var triggerName = RowVersionTriggerFormat.Fmt(modelDef.ModelName);
                var triggerBody = "SET NEW.{0} = OLD.{0} + 1;".Fmt(
                    modelDef.RowVersion.FieldName.SqlColumn(this));

                var sql = "CREATE TRIGGER {0} BEFORE UPDATE ON {1} FOR EACH ROW BEGIN {2} END;".Fmt(
                    triggerName, GetTableName(modelDef), triggerBody);

                return sql;
            }

            return null;
        }

        public override string GetQuotedValue(string paramValue)
        {
            return "'" + paramValue.Replace("\\", "\\\\").Replace("'", @"\'") + "'";
        }

        public override string GetQuotedValue(object value, Type fieldType)
        {
            if (value == null)
                return "NULL";

            if (fieldType == typeof(byte[]))
                return "0x" + BitConverter.ToString((byte[])value).Replace("-", "");

            return base.GetQuotedValue(value, fieldType);
        }

        public override string GetTableName(string table, string schema = null) =>
            GetTableName(table, schema, useStrategy:true);

        public override string GetTableName(string table, string schema, bool useStrategy)
        {
            if (useStrategy)
            {
                return schema != null && !table.StartsWithIgnoreCase(schema + "_")
                    ? QuoteIfRequired(NamingStrategy.GetSchemaName(schema) + "_" + NamingStrategy.GetTableName(table))
                    : QuoteIfRequired(NamingStrategy.GetTableName(table));
            }

            return schema != null && !table.StartsWithIgnoreCase(schema + "_")
                ? QuoteIfRequired(schema + "_" + table)
                : QuoteIfRequired(table);
        }

        public override bool ShouldQuote(string name) => name != null &&
            (ReservedWords.Contains(name) || name.IndexOf(' ') >= 0 || name.IndexOf('.') >= 0);

        public override string GetQuotedName(string name) => name == null ? null : name.FirstCharEquals('`')
            ? name : '`' + name + '`';

        public override string GetQuotedTableName(string tableName, string schema = null)
        {
            return GetQuotedName(GetTableName(tableName, schema));
        }

        public override SqlExpression<T> SqlExpression<T>()
        {
            return new MySqlExpression<T>(this);
        }

        public override string ToTableNamesStatement(string schema)
        {
            return schema == null
                ? "SELECT table_name FROM information_schema.tables WHERE table_type='BASE TABLE' AND table_schema = DATABASE()"
                : "SELECT table_name FROM information_schema.tables WHERE table_type='BASE TABLE' AND table_schema = DATABASE() AND table_name LIKE {0}".SqlFmt(this, NamingStrategy.GetSchemaName(schema)  + "\\_%");
        }

        public override string ToTableNamesWithRowCountsStatement(bool live, string schema)
        {
            if (live)
                return null;

            return schema == null
                ? "SELECT table_name, table_rows FROM information_schema.tables WHERE table_type='BASE TABLE' AND table_schema = DATABASE()"
                : "SELECT table_name, table_rows FROM information_schema.tables WHERE table_type='BASE TABLE' AND table_schema = DATABASE() AND table_name LIKE {0}".SqlFmt(this, NamingStrategy.GetSchemaName(schema)  + "\\_%");
        }

        public override bool DoesTableExist(IDbCommand dbCmd, string tableName, string schema = null)
        {
            var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0} AND TABLE_SCHEMA = {1}"
                .SqlFmt(GetTableName(tableName, schema).StripDbQuotes(), dbCmd.Connection.Database);

            var result = dbCmd.ExecLongScalar(sql);

            return result > 0;
        }

        public override async Task<bool> DoesTableExistAsync(IDbCommand dbCmd, string tableName, string schema = null, CancellationToken token=default)
        {
            var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0} AND TABLE_SCHEMA = {1}"
                .SqlFmt(GetTableName(tableName, schema).StripDbQuotes(), dbCmd.Connection.Database);

            var result = await dbCmd.ExecLongScalarAsync(sql, token);

            return result > 0;
        }

        public override bool DoesColumnExist(IDbConnection db, string columnName, string tableName, string schema = null)
        {
            tableName = GetTableName(tableName, schema).StripQuotes();
            var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS"
                      + " WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName AND TABLE_SCHEMA = @schema"
                          .SqlFmt(GetTableName(tableName, schema).StripDbQuotes(), columnName);

            var result = db.SqlScalar<long>(sql, new { tableName, columnName, schema = db.Database });

            return result > 0;
        }

        public override async Task<bool> DoesColumnExistAsync(IDbConnection db, string columnName, string tableName, string schema = null, CancellationToken token=default)
        {
            tableName = GetTableName(tableName, schema).StripQuotes();
            var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS"
                      + " WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName AND TABLE_SCHEMA = @schema"
                          .SqlFmt(GetTableName(tableName, schema).StripDbQuotes(), columnName);

            var result = await db.SqlScalarAsync<long>(sql, new { tableName, columnName, schema = db.Database }, token);

            return result > 0;
        }

        public override string ToCreateTableStatement(Type tableType)
        {
            var sbColumns = StringBuilderCache.Allocate();
            var sbConstraints = StringBuilderCache.Allocate();

            var modelDef = GetModel(tableType);
            foreach (var fieldDef in CreateTableFieldsStrategy(modelDef))
            {
                if (fieldDef.CustomSelect != null || (fieldDef.IsComputed && !fieldDef.IsPersisted))
                    continue;

                if (sbColumns.Length != 0) sbColumns.Append(", \n  ");

                sbColumns.Append(GetColumnDefinition(fieldDef, modelDef));

                var sqlConstraint = GetCheckConstraint(modelDef, fieldDef);
                if (sqlConstraint != null)
                {
                    sbConstraints.Append(",\n" + sqlConstraint);
                }

                if (fieldDef.ForeignKey == null || OrmLiteConfig.SkipForeignKeys)
                    continue;

                var refModelDef = GetModel(fieldDef.ForeignKey.ReferenceType);
                sbConstraints.AppendFormat(
                    ", \n\n  CONSTRAINT {0} FOREIGN KEY ({1}) REFERENCES {2} ({3})",
                    GetQuotedName(fieldDef.ForeignKey.GetForeignKeyName(modelDef, refModelDef, NamingStrategy, fieldDef)),
                    GetQuotedColumnName(fieldDef.FieldName),
                    GetQuotedTableName(refModelDef),
                    GetQuotedColumnName(refModelDef.PrimaryKey.FieldName));

                if (!string.IsNullOrEmpty(fieldDef.ForeignKey.OnDelete))
                    sbConstraints.AppendFormat(" ON DELETE {0}", fieldDef.ForeignKey.OnDelete);

                if (!string.IsNullOrEmpty(fieldDef.ForeignKey.OnUpdate))
                    sbConstraints.AppendFormat(" ON UPDATE {0}", fieldDef.ForeignKey.OnUpdate);
            }

            var uniqueConstraints = GetUniqueConstraints(modelDef);
            if (uniqueConstraints != null)
            {
                sbConstraints.Append(",\n" + uniqueConstraints);
            }

            if (modelDef.CompositePrimaryKeys.Any())
            {
                sbConstraints.Append(",\n");

                sbConstraints.Append(" PRIMARY KEY (");

                sbConstraints.Append(
                    modelDef.CompositePrimaryKeys.FirstOrDefault().FieldNames.Map(f => modelDef.GetQuotedName(f, this))
                        .Join(","));

                sbConstraints.Append(") ");
            }

            var sql = $"CREATE TABLE {GetQuotedTableName(modelDef)} \n(\n  {StringBuilderCache.ReturnAndFree(sbColumns)}{StringBuilderCacheAlt.ReturnAndFree(sbConstraints)} \n); \n";

            return sql;
        }

        public override bool DoesSchemaExist(IDbCommand dbCmd, string schemaName)
        {
            // schema is prefixed to table name
            return true;
        }

        public override string ToCreateSchemaStatement(string schemaName)
        {
            // https://mariadb.com/kb/en/library/create-database/
            return $"SELECT 1";
        }

        public override string GetColumnDefinition(FieldDefinition fieldDef)
        {
            if (fieldDef.PropertyInfo?.HasAttributeCached<TextAttribute>() == true)
            {
                var sql = StringBuilderCache.Allocate();
                sql.AppendFormat("{0} {1}", GetQuotedName(NamingStrategy.GetColumnName(fieldDef.FieldName)), TextColumnDefinition);
                sql.Append(fieldDef.IsNullable ? " NULL" : " NOT NULL");
                return StringBuilderCache.ReturnAndFree(sql);
            }

            var ret = base.GetColumnDefinition(fieldDef);
            if (fieldDef.IsRowVersion)
                return $"{ret} DEFAULT 1";

            return ret;
        }

        public override string GetColumnDefinition(FieldDefinition fieldDef, ModelDefinition modelDef)
        {
            if (fieldDef.PropertyInfo?.HasAttributeCached<TextAttribute>() == true)
            {
                var sql = StringBuilderCache.Allocate();
                sql.AppendFormat("{0} {1}", this.GetQuotedName(this.NamingStrategy.GetColumnName(fieldDef.FieldName)), TextColumnDefinition);
                sql.Append(fieldDef.IsNullable ? " NULL" : " NOT NULL");
                return StringBuilderCache.ReturnAndFree(sql);
            }

            var ret = base.GetColumnDefinition(fieldDef, modelDef);

            return fieldDef.IsRowVersion ? $"{ret} DEFAULT 1" : ret;
        }

        public override string SqlConflict(string sql, string conflictResolution)
        {
            var parts = sql.SplitOnFirst(' ');
            return $"{parts[0]} {conflictResolution} {parts[1]}";
        }

        public override string SqlCurrency(string fieldOrValue, string currencySymbol) =>
            SqlConcat(new[] {$"'{currencySymbol}'", $"cast({fieldOrValue} as decimal(15,2))"});

        public override string SqlCast(object fieldOrValue, string castAs) =>
            castAs == Sql.VARCHAR
                ? $"CAST({fieldOrValue} AS CHAR(1000))"
                : $"CAST({fieldOrValue} AS {castAs})";

        public override string SqlBool(bool value) => value ? "1" : "0";

        public override void EnableForeignKeysCheck(IDbCommand cmd) => cmd.ExecNonQuery("SET FOREIGN_KEY_CHECKS=1;");
        public override Task EnableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default) =>
            cmd.ExecNonQueryAsync("SET FOREIGN_KEY_CHECKS=1;", null, token);
        public override void DisableForeignKeysCheck(IDbCommand cmd) => cmd.ExecNonQuery("SET FOREIGN_KEY_CHECKS=0;");
        public override Task DisableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default) =>
            cmd.ExecNonQueryAsync("SET FOREIGN_KEY_CHECKS=0;", null, token);

        protected DbConnection Unwrap(IDbConnection db)
        {
            return (DbConnection)db.ToDbConnection();
        }

        protected DbCommand Unwrap(IDbCommand cmd)
        {
            return (DbCommand)cmd.ToDbCommand();
        }

        protected DbDataReader Unwrap(IDataReader reader)
        {
            return (DbDataReader)reader;
        }

#if ASYNC
        public override Task OpenAsync(IDbConnection db, CancellationToken token = default)
        {
            return Unwrap(db).OpenAsync(token);
        }

        public override Task<IDataReader> ExecuteReaderAsync(IDbCommand cmd, CancellationToken token = default)
        {
            return Unwrap(cmd).ExecuteReaderAsync(token).Then(x => (IDataReader)x);
        }

        public override Task<int> ExecuteNonQueryAsync(IDbCommand cmd, CancellationToken token = default)
        {
            return Unwrap(cmd).ExecuteNonQueryAsync(token);
        }

        public override Task<object> ExecuteScalarAsync(IDbCommand cmd, CancellationToken token = default)
        {
            return Unwrap(cmd).ExecuteScalarAsync(token);
        }

        public override Task<bool> ReadAsync(IDataReader reader, CancellationToken token = default)
        {
            return Unwrap(reader).ReadAsync(token);
        }

        public override async Task<List<T>> ReaderEach<T>(IDataReader reader, Func<T> fn, CancellationToken token = default)
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

        public override async Task<Return> ReaderEach<Return>(IDataReader reader, Action fn, Return source, CancellationToken token = default)
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

        public override async Task<T> ReaderRead<T>(IDataReader reader, Func<T> fn, CancellationToken token = default)
        {
            try
            {
                if (await ReadAsync(reader, token).ConfigureAwait(false))
                    return fn();

                return default(T);
            }
            finally
            {
                reader.Dispose();
            }
        }
#endif

        public override string GetDropFunction(string database, string functionName)
        {
            var sb = StringBuilderCache.Allocate();

            var tableName = $"{database}.{base.NamingStrategy.GetTableName(functionName)}";

            sb.Append("DROP FUNCTION IF EXISTS ");
            sb.AppendFormat("{0}", tableName);

            sb.Append(";");

            return StringBuilderCache.ReturnAndFree(sb);
        }

        public override string GetCreateView(string database, ModelDefinition modelDef, StringBuilder selectSql)
        {
            var sb = StringBuilderCache.Allocate();

            var tableName = $"{database}.{base.NamingStrategy.GetTableName(modelDef.ModelName)}";

            sb.AppendFormat("CREATE VIEW {0} as ", tableName);

            sb.Append(selectSql);

            sb.Append(";");

            return StringBuilderCache.ReturnAndFree(sb);
        }

        public override string GetDropView(string database, ModelDefinition modelDef)
        {
            var sb = StringBuilderCache.Allocate();

            var tableName = $"{database}.{base.NamingStrategy.GetTableName(modelDef.ModelName)}";

            sb.Append("DROP VIEW IF EXISTS ");
            sb.AppendFormat("{0}", tableName);

            sb.Append(";");

            return StringBuilderCache.ReturnAndFree(sb);
        }

        public override string GetUtcDateFunction()
        {
            return "UTC_DATE()";
        }

        public override string DateDiffFunction(string interval, string date1, string date2)
        {
            return $"DATEDIFF({date1}, {date2})";
        }

        /// <summary>
        /// Gets the SQL ISNULL Function
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="alternateValue">
        /// The alternate Value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string IsNullFunction(string expression, object alternateValue)
        {
            return $"IFNULL(({expression}), {alternateValue})";
        }

        public override string ConvertFlag(string expression)
        {
            return $"cast(sign({expression}) as signed)";
        }

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

        public override string DatabaseSize(string database)
        {
            return $"SELECT sum( data_length + index_length ) / 1024 / 1024 FROM information_schema.TABLES WHERE table_schema = '{database}'";
        }

        public override string SQLVersion()
        {
            return "select @@version";
        }

        public override string SQLServerName()
        {
            return "MySQL";
        }

        public override string ShrinkDatabase(string database)
        {
            return ""; //$"optimize table {database}";
        }

        public override string ReIndexDatabase(string database, string objectQualifier)
        {
            return ""; //$"REINDEX DATABASE {database}";
        }

        public override string ChangeRecoveryMode(string database, string mode)
        {
            return "";
        }

        /// <summary>
        /// Just runs the SQL command according to specifications.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// Returns the Results
        /// </returns>
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
    }
}