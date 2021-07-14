using System;
using System.Collections.Generic;
using System.Data;
#if MSDATA
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite.SqlServer.Converters;
using ServiceStack.Text;
#if NETSTANDARD2_0 || NET5_0
using ApplicationException = System.InvalidOperationException;
#endif

namespace ServiceStack.OrmLite.SqlServer
{
    using System.Text;

    public class SqlServerOrmLiteDialectProvider : OrmLiteDialectProviderBase<SqlServerOrmLiteDialectProvider>
    {
        public static SqlServerOrmLiteDialectProvider Instance = new SqlServerOrmLiteDialectProvider();

        public SqlServerOrmLiteDialectProvider()
        {
            this.AutoIncrementDefinition = "IDENTITY(1,1)";
            base.SelectIdentitySql = "SELECT SCOPE_IDENTITY()";

            this.InitColumnTypeMap();

            this.RowVersionConverter = new SqlServerRowVersionConverter();

            this.RegisterConverter<string>(new SqlServerStringConverter());
            this.RegisterConverter<bool>(new SqlServerBoolConverter());

            this.RegisterConverter<sbyte>(new SqlServerSByteConverter());
            this.RegisterConverter<ushort>(new SqlServerUInt16Converter());
            this.RegisterConverter<uint>(new SqlServerUInt32Converter());
            this.RegisterConverter<ulong>(new SqlServerUInt64Converter());

            this.RegisterConverter<float>(new SqlServerFloatConverter());
            this.RegisterConverter<double>(new SqlServerDoubleConverter());
            this.RegisterConverter<decimal>(new SqlServerDecimalConverter());

            this.RegisterConverter<DateTime>(new SqlServerDateTimeConverter());

            this.RegisterConverter<Guid>(new SqlServerGuidConverter());

            this.RegisterConverter<byte[]>(new SqlServerByteArrayConverter());

            this.Variables = new Dictionary<string, string>
            {
                { OrmLiteVariables.SystemUtc, "SYSUTCDATETIME()" },
                { OrmLiteVariables.MaxText, "VARCHAR(MAX)" },
                { OrmLiteVariables.MaxTextUnicode, "NVARCHAR(MAX)" },
                { OrmLiteVariables.True, this.SqlBool(true) },
                { OrmLiteVariables.False, this.SqlBool(false) },
            };
        }

        public override string GetQuotedValue(string paramValue)
        {
            return (this.StringConverter.UseUnicode ? "N'" : "'") + paramValue.Replace("'", "''") + "'";
        }

        public override IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options)
        {
            var isFullConnectionString = connectionString.Contains(";");

            if (!isFullConnectionString)
            {
                var filePath = connectionString;

                var filePathWithExt = filePath.EndsWithIgnoreCase(".mdf") ? filePath : filePath + ".mdf";

                var fileName = Path.GetFileName(filePathWithExt);
                var dbName = fileName.Substring(0, fileName.Length - ".mdf".Length);

                connectionString =
                    $@"Data Source=.\SQLEXPRESS;AttachDbFilename={filePathWithExt};Initial Catalog={dbName};Integrated Security=True;User Instance=True;";
            }

            if (options != null)
            {
                foreach (var option in options)
                {
                    if (option.Key.ToLower() == "read only")
                    {
                        if (option.Value.ToLower() == "true")
                        {
                            connectionString += "Mode = Read Only;";
                        }

                        continue;
                    }

                    connectionString += option.Key + "=" + option.Value + ";";
                }
            }

            return new SqlConnection(connectionString);
        }

        public override SqlExpression<T> SqlExpression<T>() => new SqlServerExpression<T>(this);

        public override IDbDataParameter CreateParam() => new SqlParameter();

        private const string DefaultSchema = "dbo";

        public override string ToTableNamesStatement(string schema)
        {
            var sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";
            return sql + " AND TABLE_SCHEMA = {0}".SqlFmt(this, schema ?? DefaultSchema);
        }

        public override string ToTableNamesWithRowCountsStatement(bool live, string schema)
        {
            var schemaSql = " AND s.Name = {0}".SqlFmt(this, schema ?? DefaultSchema);

            var sql = @"SELECT t.NAME, p.rows FROM sys.tables t INNER JOIN sys.schemas s ON t.schema_id = s.schema_id 
                               INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id 
                               INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
                         WHERE t.is_ms_shipped = 0 " + schemaSql + " GROUP BY t.NAME, p.Rows";
            return sql;
        }

        public override bool DoesSchemaExist(IDbCommand dbCmd, string schemaName)
        {
            var sql = $"SELECT count(*) FROM sys.schemas WHERE name = '{schemaName.SqlParam()}'";
            var result = dbCmd.ExecLongScalar(sql);
            return result > 0;
        }

        public override async Task<bool> DoesSchemaExistAsync(
            IDbCommand dbCmd,
            string schemaName,
            CancellationToken token = default)
        {
            var sql = $"SELECT count(*) FROM sys.schemas WHERE name = '{schemaName.SqlParam()}'";
            var result = await dbCmd.ExecLongScalarAsync(sql, token);
            return result > 0;
        }

        public override string ToCreateSchemaStatement(string schemaName)
        {
            var sql = $"CREATE SCHEMA [{this.GetSchemaName(schemaName)}]";
            return sql;
        }

        public override bool DoesTableExist(IDbCommand dbCmd, string tableName, string schema = null)
        {
            var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}".SqlFmt(this, tableName);

            if (schema != null)
                sql += " AND TABLE_SCHEMA = {0}".SqlFmt(this, schema);
            else
                sql += " AND TABLE_SCHEMA <> 'Security'";

            var result = dbCmd.ExecLongScalar(sql);

            return result > 0;
        }

        public override async Task<bool> DoesTableExistAsync(
            IDbCommand dbCmd,
            string tableName,
            string schema = null,
            CancellationToken token = default)
        {
            var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}".SqlFmt(this, tableName);

            if (schema != null)
                sql += " AND TABLE_SCHEMA = {0}".SqlFmt(this, schema);
            else
                sql += " AND TABLE_SCHEMA <> 'Security'";

            var result = await dbCmd.ExecLongScalarAsync(sql, token);

            return result > 0;
        }

        public override bool DoesColumnExist(
            IDbConnection db,
            string columnName,
            string tableName,
            string schema = null)
        {
            var sql =
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName"
                    .SqlFmt(this, tableName, columnName);

            if (schema != null)
                sql += " AND TABLE_SCHEMA = @schema";

            var result = db.SqlScalar<long>(sql, new { tableName, columnName, schema });

            return result > 0;
        }

        public override async Task<bool> DoesColumnExistAsync(
            IDbConnection db,
            string columnName,
            string tableName,
            string schema = null,
            CancellationToken token = default)
        {
            var sql =
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName"
                    .SqlFmt(this, tableName, columnName);

            if (schema != null)
                sql += " AND TABLE_SCHEMA = @schema";

            var result = await db.SqlScalarAsync<long>(sql, new { tableName, columnName, schema }, token: token);

            return result > 0;
        }

        public override string GetColumnDataType(
            IDbConnection db,
            string columnName,
            string tableName,
            string schema = null)
        {
            var sql =
                "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName"
                    .SqlFmt(this, tableName, columnName);

            if (schema != null)
            {
                sql += " AND TABLE_SCHEMA = @schema";
            }

            return db.SqlScalar<string>(sql, new { tableName, columnName, schema });
        }

        public override bool ColumnIsNullable(
            IDbConnection db,
            string columnName,
            string tableName,
            string schema = null)
        {
            var sql =
                "SELECT is_nullable FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName"
                    .SqlFmt(this, tableName, columnName);

            if (schema != null)
            {
                sql += " AND TABLE_SCHEMA = @schema";
            }

            return db.SqlScalar<string>(sql, new { tableName, columnName, schema }) == "YES";
        }

        public override long GetColumnMaxLength(
            IDbConnection db,
            string columnName,
            string tableName,
            string schema = null)
        {
            var sql =
                "SELECT character_maximum_length FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName"
                    .SqlFmt(this, tableName, columnName);

            if (schema != null)
                sql += " AND TABLE_SCHEMA = @schema";

            return db.SqlScalar<long>(sql, new { tableName, columnName, schema });
        }

        public override string GetForeignKeyOnDeleteClause(ForeignKeyConstraint foreignKey)
        {
            return "RESTRICT" == (foreignKey.OnDelete ?? string.Empty).ToUpper()
                ? string.Empty
                : base.GetForeignKeyOnDeleteClause(foreignKey);
        }

        public override string GetForeignKeyOnUpdateClause(ForeignKeyConstraint foreignKey)
        {
            return (foreignKey.OnUpdate ?? string.Empty).ToUpper() == "RESTRICT"
                ? string.Empty
                : base.GetForeignKeyOnUpdateClause(foreignKey);
        }

        public override string GetDropFunction(string database, string functionName)
        {
            var sb = StringBuilderCache.Allocate();

            var tableName = this.GetTableNameWithBrackets(functionName);

            sb.Append("IF EXISTS (");
            sb.Append("SELECT top 1 1 FROM sys.objects WHERE ");
            sb.AppendFormat("object_id = OBJECT_ID(N'{0}')", tableName);
            sb.Append("and type in (N'FN', N'IF', N'TF') )");
            sb.Append(" begin");
            sb.AppendFormat(" drop function {0}", tableName);
            sb.Append(" end");

            return StringBuilderCache.ReturnAndFree(sb);
        }

        public override string GetCreateView(string database, ModelDefinition modelDef, StringBuilder selectSql)
        {
            var sb = StringBuilderCache.Allocate();

            var tableName = this.GetTableNameWithBrackets(modelDef);

            sb.Append("IF NOT EXISTS (");
            sb.Append("SELECT top 1 1 FROM sys.objects WHERE ");
            sb.AppendFormat("object_id = OBJECT_ID(N'{0}')", tableName);
            sb.Append("and type in (N'V'))");
            sb.Append(" begin ");

            sb.AppendFormat("EXEC sys.sp_executesql @statement = N'CREATE VIEW {0}", tableName);
            sb.Append(" WITH SCHEMABINDING");
            sb.Append(" AS");

            sb.Append(selectSql);

            sb.Append("'");

            sb.Append(" end");

            return StringBuilderCache.ReturnAndFree(sb);
        }

        public override string GetDropView(string database, ModelDefinition modelDef)
        {
            var sb = StringBuilderCache.Allocate();

            var tableName = this.GetTableNameWithBrackets(modelDef);

            sb.Append("IF EXISTS (");
            sb.Append("SELECT top 1 1 FROM sys.objects WHERE ");
            sb.AppendFormat("object_id = OBJECT_ID(N'{0}')", tableName);
            sb.Append("and type in (N'V'))");
            sb.Append(" begin");
            sb.AppendFormat(" drop view {0}", tableName);
            sb.Append(" end");

            return StringBuilderCache.ReturnAndFree(sb);
        }

        public override string GetCreateIndexView(ModelDefinition modelDef, string name, string selectSql)
        {
            var sb = StringBuilderCache.Allocate();

            var indexName = $"{this.NamingStrategy.GetTableName(modelDef)}_{name}";

            var tableName = this.GetTableNameWithBrackets(modelDef);

            sb.Append("IF NOT EXISTS (");
            sb.Append("SELECT top 1 1 FROM sys.indexes WHERE ");
            sb.AppendFormat("object_id = OBJECT_ID(N'{0}')", tableName);
            sb.AppendFormat("and name = N'{0}')", indexName);
            sb.Append(" BEGIN");

            sb.Append(" SET ARITHABORT ON ");

            sb.AppendFormat("CREATE UNIQUE CLUSTERED INDEX [{0}] ", indexName);
            sb.AppendFormat("ON {0}", tableName);

            sb.Append(" ( ");

            sb.Append(selectSql);

            sb.Append(
                ") WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ");
            sb.Append("DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ");

            sb.Append(" END");

            return StringBuilderCache.ReturnAndFree(sb);
        }

        public override string GetDropIndexView(ModelDefinition modelDef, string name)
        {
            var sb = StringBuilderCache.Allocate();

            var indexName = $"{this.NamingStrategy.GetTableName(modelDef)}_{name}";

            var tableName = this.GetTableNameWithBrackets(modelDef);

            sb.Append("IF EXISTS (");
            sb.Append("SELECT top 1 1 FROM sys.indexes WHERE ");
            sb.AppendFormat("object_id = OBJECT_ID(N'{0}')", tableName);
            sb.AppendFormat("and name = N'{0}')", indexName);
            sb.Append(" BEGIN");
            sb.AppendFormat(" drop index [{0}] on {1}", indexName, tableName);
            sb.Append(" END");

            return StringBuilderCache.ReturnAndFree(sb);
        }

        public override string GetDropIndexConstraint(ModelDefinition modelDef, string name = null)
        {
            var sb = StringBuilderCache.Allocate();

            var indexName = name.IsNullOrEmpty() ? $"IX_{this.NamingStrategy.GetTableName(modelDef)}" : name;

            var tableName = this.GetQuotedTableName(modelDef);

            sb.Append("IF EXISTS (");
            sb.Append("SELECT top 1 1 FROM sys.indexes WHERE ");
            sb.AppendFormat(
                "object_id = OBJECT_ID(N'[{0}].[{1}]')",
                this.NamingStrategy.GetSchemaName(modelDef),
                this.NamingStrategy.GetTableName(modelDef));
            sb.AppendFormat("and name = N'{0}')", indexName);
            sb.Append(" BEGIN");
            sb.AppendFormat("  ALTER TABLE {1} DROP constraint {0}", indexName, tableName);
            sb.Append(" END");

            return StringBuilderCache.ReturnAndFree(sb);
        }

        public override string GetDropPrimaryKeyConstraint(ModelDefinition modelDef, string name)
        {
            var sb = StringBuilderCache.Allocate();

            var foreignKeyName = $"PK_{this.NamingStrategy.GetTableName(modelDef)}_{name}";

            var tableName = this.GetQuotedTableName(modelDef);

            sb.Append("IF EXISTS (");
            sb.Append("SELECT top 1 1 FROM sys.foreign_keys WHERE ");
            sb.AppendFormat(
                "object_id = OBJECT_ID(N'[{0}].[{1}]')",
                this.NamingStrategy.GetSchemaName(modelDef),
                this.NamingStrategy.GetTableName(modelDef));
            sb.AppendFormat("and name = N'{0}')", foreignKeyName);
            sb.Append(" BEGIN");
            sb.AppendFormat("  ALTER TABLE {1} DROP constraint {0}", foreignKeyName, tableName);
            sb.Append(" END");

            return StringBuilderCache.ReturnAndFree(sb);
        }

        public override string GetDropForeignKeyConstraint(ModelDefinition modelDef, string name)
        {
            var sb = StringBuilderCache.Allocate();

            var foreignKeyName = $"FK_{this.NamingStrategy.GetTableName(modelDef)}_{name}";

            var tableName = this.GetQuotedTableName(modelDef);

            sb.Append("IF EXISTS (");
            sb.Append("SELECT top 1 1 FROM sys.foreign_keys WHERE ");
            sb.AppendFormat(
                "object_id = OBJECT_ID(N'[{0}].[{1}]')",
                this.NamingStrategy.GetSchemaName(modelDef),
                this.NamingStrategy.GetTableName(modelDef));
            sb.AppendFormat("and name = N'{0}')", foreignKeyName);
            sb.Append("BEGIN");
            sb.AppendFormat("  ALTER TABLE {1} DROP constraint {0}", foreignKeyName, tableName);
            sb.Append(" END");

            return StringBuilderCache.ReturnAndFree(sb);
        }

        public override string GetDropForeignKeyConstraints(ModelDefinition modelDef)
        {
            // TODO: find out if this should go in base class?
            var sb = StringBuilderCache.Allocate();
            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (fieldDef.ForeignKey != null)
                {
                    var foreignKeyName = fieldDef.ForeignKey.GetForeignKeyName(
                        modelDef,
                        OrmLiteUtils.GetModelDefinition(fieldDef.ForeignKey.ReferenceType),
                        this.NamingStrategy,
                        fieldDef);

                    var tableName = this.GetQuotedTableName(modelDef);
                    sb.AppendLine($"IF EXISTS (SELECT name FROM sys.foreign_keys WHERE name = '{foreignKeyName}')");
                    sb.AppendLine("BEGIN");
                    sb.AppendLine($"  ALTER TABLE {tableName} DROP {foreignKeyName};");
                    sb.AppendLine("END");
                }
            }

            return StringBuilderCache.ReturnAndFree(sb);
        }

        public override string ToAddColumnStatement(Type modelType, FieldDefinition fieldDef)
        {
            var column = this.GetColumnDefinition(fieldDef);
            var modelName = this.GetQuotedTableName(GetModel(modelType));

            return $"ALTER TABLE {modelName} ADD {column};";
        }

        public override string ToAlterColumnStatement(Type modelType, FieldDefinition fieldDef)
        {
            var column = this.GetColumnDefinition(fieldDef);
            var modelName = this.GetQuotedTableName(GetModel(modelType));

            return $"ALTER TABLE {modelName} ALTER COLUMN {column};";
        }

        public override string ToChangeColumnNameStatement(
            Type modelType,
            FieldDefinition fieldDef,
            string oldColumnName)
        {
            var modelName = this.NamingStrategy.GetTableName(GetModel(modelType));
            var objectName = $"{modelName}.{oldColumnName}";

            return
                $"EXEC sp_rename {this.GetQuotedValue(objectName)}, {this.GetQuotedValue(fieldDef.FieldName)}, {this.GetQuotedValue("COLUMN")};";
        }

        protected virtual string GetAutoIncrementDefinition(FieldDefinition fieldDef)
        {
            return this.AutoIncrementDefinition;
        }

        public override string GetAutoIdDefaultValue(FieldDefinition fieldDef)
        {
            return fieldDef.FieldType == typeof(Guid) ? "newid()" : null;
        }

        public override string GetColumnDefinition(FieldDefinition fieldDef)
        {
            // https://msdn.microsoft.com/en-us/library/ms182776.aspx
            if (fieldDef.IsRowVersion)
                return $"{fieldDef.FieldName} rowversion NOT NULL";

            var fieldDefinition = this.ResolveFragment(fieldDef.CustomFieldDefinition) ?? this.GetColumnTypeDefinition(
                fieldDef.ColumnType,
                fieldDef.FieldLength,
                fieldDef.Scale);

            var sql = StringBuilderCache.Allocate();
            sql.Append($"{this.GetQuotedColumnName(fieldDef.FieldName)} {fieldDefinition}");

            if (fieldDef.FieldType == typeof(string))
            {
                // https://msdn.microsoft.com/en-us/library/ms184391.aspx
                var collation = fieldDef.PropertyInfo?.FirstAttribute<SqlServerCollateAttribute>()?.Collation;
                if (!string.IsNullOrEmpty(collation))
                {
                    sql.Append($" COLLATE {collation}");
                }
            }

            if (fieldDef.IsPrimaryKey)
            {
                sql.Append(" PRIMARY KEY");

                if (fieldDef.IsNonClustered)
                    sql.Append(" NONCLUSTERED");

                if (fieldDef.AutoIncrement)
                {
                    sql.Append(" ").Append(this.GetAutoIncrementDefinition(fieldDef));
                }
            }
            else
            {
                sql.Append(fieldDef.IsNullable ? " NULL" : " NOT NULL");
            }

            if (fieldDef.IsUniqueConstraint)
            {
                sql.Append(" UNIQUE");
            }

            var defaultValue = this.GetDefaultValue(fieldDef);
            if (!string.IsNullOrEmpty(defaultValue))
            {
                sql.AppendFormat(this.DefaultValueFormat, defaultValue);
            }

            return StringBuilderCache.ReturnAndFree(sql);
        }

        public override string GetColumnDefinition(FieldDefinition fieldDef, ModelDefinition modelDef)
        {
            // https://msdn.microsoft.com/en-us/library/ms182776.aspx
            if (fieldDef.IsRowVersion)
                return $"{fieldDef.FieldName} rowversion NOT NULL";

            var fieldDefinition = this.ResolveFragment(fieldDef.CustomFieldDefinition) ?? this.GetColumnTypeDefinition(
                fieldDef.ColumnType,
                fieldDef.FieldLength,
                fieldDef.Scale);

            var sql = StringBuilderCache.Allocate();
            sql.Append($"{this.GetQuotedColumnName(fieldDef.FieldName)} {fieldDefinition}");

            if (fieldDef.FieldType == typeof(string))
            {
                // https://msdn.microsoft.com/en-us/library/ms184391.aspx
                var collation = fieldDef.PropertyInfo?.FirstAttribute<SqlServerCollateAttribute>()?.Collation;
                if (!string.IsNullOrEmpty(collation))
                {
                    sql.Append($" COLLATE {collation}");
                }
            }

            if (fieldDef.IsPrimaryKey)
            {
                sql.Append(" PRIMARY KEY");

                if (fieldDef.IsNonClustered)
                    sql.Append(" NONCLUSTERED");

                if (fieldDef.AutoIncrement)
                {
                    sql.Append(" ").Append(this.GetAutoIncrementDefinition(fieldDef));
                }
            }
            else
            {
                sql.Append(fieldDef.IsNullable ? " NULL" : " NOT NULL");
            }

            if (fieldDef.IsUniqueConstraint)
            {
                sql.Append(" UNIQUE");
            }

            var defaultValue = this.GetDefaultValue(fieldDef);
            if (!string.IsNullOrEmpty(defaultValue))
            {
                sql.AppendFormat(this.DefaultValueFormat, defaultValue);
            }

            return StringBuilderCache.ReturnAndFree(sql);
        }

        public override string ToInsertRowStatement(
            IDbCommand cmd,
            object objWithProperties,
            ICollection<string> insertFields = null)
        {
            var sbColumnNames = StringBuilderCache.Allocate();
            var sbColumnValues = StringBuilderCacheAlt.Allocate();
            var sbReturningColumns = StringBuilderCacheAlt.Allocate();
            var tableType = objWithProperties.GetType();
            var modelDef = GetModel(tableType);

            var fieldDefs = this.GetInsertFieldDefinitions(modelDef, insertFields);
            foreach (var fieldDef in fieldDefs)
            {
                if (this.ShouldReturnOnInsert(modelDef, fieldDef))
                {
                    if (sbReturningColumns.Length > 0)
                        sbReturningColumns.Append(",");
                    sbReturningColumns.Append("INSERTED." + this.GetQuotedColumnName(fieldDef.FieldName));
                }

                if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
                    continue;

                if (sbColumnNames.Length > 0)
                    sbColumnNames.Append(",");
                if (sbColumnValues.Length > 0)
                    sbColumnValues.Append(",");

                try
                {
                    sbColumnNames.Append(this.GetQuotedColumnName(fieldDef.FieldName));
                    sbColumnValues.Append(this.GetParam(this.SanitizeFieldNameForParamName(fieldDef.FieldName)));

                    AddParameter(cmd, fieldDef);
                }
                catch (Exception ex)
                {
                    Log.Error("ERROR in ToInsertRowStatement(): " + ex.Message, ex);
                    throw;
                }
            }

            foreach (var fieldDef in modelDef.AutoIdFields)
            {
                // need to include any AutoId fields that weren't included
                if (fieldDefs.Contains(fieldDef))
                    continue;

                if (sbReturningColumns.Length > 0)
                    sbReturningColumns.Append(",");
                sbReturningColumns.Append("INSERTED." + this.GetQuotedColumnName(fieldDef.FieldName));
            }

            var strReturning = StringBuilderCacheAlt.ReturnAndFree(sbReturningColumns);
            strReturning = strReturning.Length > 0 ? "OUTPUT " + strReturning + " " : string.Empty;
            var sql = sbColumnNames.Length > 0
                ? $"INSERT INTO {this.GetQuotedTableName(modelDef)} ({StringBuilderCache.ReturnAndFree(sbColumnNames)}) " +
                  strReturning + $"VALUES ({StringBuilderCacheAlt.ReturnAndFree(sbColumnValues)})"
                : $"INSERT INTO {this.GetQuotedTableName(modelDef)} {strReturning} DEFAULT VALUES";

            return sql;
        }

        protected string Sequence(string schema, string sequence)
        {
            /*if (schema == null)
                return this.GetQuotedName(sequence);*/

            var escapedSchema = this.NamingStrategy.GetSchemaName(schema).Replace(".", "\".\"");

            return this.GetQuotedName(escapedSchema) + "." + this.GetQuotedName(sequence);
        }

        protected override bool ShouldSkipInsert(FieldDefinition fieldDef) =>
            fieldDef.ShouldSkipInsert() || fieldDef.AutoId;

        protected virtual bool ShouldReturnOnInsert(ModelDefinition modelDef, FieldDefinition fieldDef) =>
            fieldDef.ReturnOnInsert ||
            fieldDef.IsPrimaryKey && fieldDef.AutoIncrement && this.HasInsertReturnValues(modelDef) || fieldDef.AutoId;

        public override bool HasInsertReturnValues(ModelDefinition modelDef) =>
            modelDef.FieldDefinitions.Any(x => x.ReturnOnInsert || x.AutoId && x.FieldType == typeof(Guid));

        protected virtual bool SupportsSequences(FieldDefinition fieldDef) => false;

        public override void EnableIdentityInsert<T>(IDbCommand cmd)
        {
            var tableName = cmd.GetDialectProvider().GetQuotedTableName(ModelDefinition<T>.Definition);
            cmd.ExecNonQuery($"SET IDENTITY_INSERT {tableName} ON");
        }

        public override Task EnableIdentityInsertAsync<T>(IDbCommand cmd, CancellationToken token = default)
        {
            var tableName = cmd.GetDialectProvider().GetQuotedTableName(ModelDefinition<T>.Definition);
            return cmd.ExecNonQueryAsync($"SET IDENTITY_INSERT {tableName} ON", null, token);
        }

        public override void DisableIdentityInsert<T>(IDbCommand cmd)
        {
            var tableName = cmd.GetDialectProvider().GetQuotedTableName(ModelDefinition<T>.Definition);
            cmd.ExecNonQuery($"SET IDENTITY_INSERT {tableName} OFF");
        }

        public override Task DisableIdentityInsertAsync<T>(IDbCommand cmd, CancellationToken token = default)
        {
            var tableName = cmd.GetDialectProvider().GetQuotedTableName(ModelDefinition<T>.Definition);
            return cmd.ExecNonQueryAsync($"SET IDENTITY_INSERT {tableName} OFF", null, token);
        }

        public override void PrepareParameterizedInsertStatement<T>(
            IDbCommand cmd,
            ICollection<string> insertFields = null,
            Func<FieldDefinition, bool> shouldInclude = null)
        {
            var sbColumnNames = StringBuilderCache.Allocate();
            var sbColumnValues = StringBuilderCacheAlt.Allocate();
            var sbReturningColumns = StringBuilderCacheAlt.Allocate();
            var modelDef = OrmLiteUtils.GetModelDefinition(typeof(T));

            cmd.Parameters.Clear();

            var fieldDefs = this.GetInsertFieldDefinitions(modelDef, insertFields);
            foreach (var fieldDef in fieldDefs)
            {
                if (this.ShouldReturnOnInsert(modelDef, fieldDef))
                {
                    if (sbReturningColumns.Length > 0)
                        sbReturningColumns.Append(",");
                    sbReturningColumns.Append("INSERTED." + this.GetQuotedColumnName(fieldDef.FieldName));
                }

                if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId && shouldInclude?.Invoke(fieldDef) != true)
                    continue;

                if (sbColumnNames.Length > 0)
                    sbColumnNames.Append(",");
                if (sbColumnValues.Length > 0)
                    sbColumnValues.Append(",");

                try
                {
                    sbColumnNames.Append(this.GetQuotedColumnName(fieldDef.FieldName));

                    if (this.SupportsSequences(fieldDef))
                    {
                        sbColumnValues.Append(
                            "NEXT VALUE FOR " + this.Sequence(
                                this.NamingStrategy.GetSchemaName(modelDef),
                                fieldDef.Sequence));
                    }
                    else
                    {
                        sbColumnValues.Append(
                            this.GetParam(
                                this.SanitizeFieldNameForParamName(fieldDef.FieldName),
                                fieldDef.CustomInsert));
                        this.AddParameter(cmd, fieldDef);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("ERROR in PrepareParameterizedInsertStatement(): " + ex.Message, ex);
                    throw;
                }
            }

            foreach (var fieldDef in modelDef.AutoIdFields)
            {
                // need to include any AutoId fields that weren't included
                if (fieldDefs.Contains(fieldDef))
                    continue;

                if (sbReturningColumns.Length > 0)
                    sbReturningColumns.Append(",");
                sbReturningColumns.Append("INSERTED." + this.GetQuotedColumnName(fieldDef.FieldName));
            }

            var strReturning = StringBuilderCacheAlt.ReturnAndFree(sbReturningColumns);
            strReturning = strReturning.Length > 0 ? "OUTPUT " + strReturning + " " : string.Empty;
            cmd.CommandText = sbColumnNames.Length > 0
                ? $"INSERT INTO {this.GetQuotedTableName(modelDef)} ({StringBuilderCache.ReturnAndFree(sbColumnNames)}) {strReturning}" +
                  $"VALUES ({StringBuilderCacheAlt.ReturnAndFree(sbColumnValues)})"
                : $"INSERT INTO {this.GetQuotedTableName(modelDef)}{strReturning} DEFAULT VALUES";
        }

        public override void PrepareInsertRowStatement<T>(IDbCommand dbCmd, Dictionary<string, object> args)
        {
            var sbColumnNames = StringBuilderCache.Allocate();
            var sbColumnValues = StringBuilderCacheAlt.Allocate();
            var sbReturningColumns = StringBuilderCacheAlt.Allocate();
            var modelDef = OrmLiteUtils.GetModelDefinition(typeof(T));

            dbCmd.Parameters.Clear();

            foreach (var entry in args)
            {
                var fieldDef = modelDef.AssertFieldDefinition(entry.Key);

                if (this.ShouldReturnOnInsert(modelDef, fieldDef))
                {
                    if (sbReturningColumns.Length > 0)
                        sbReturningColumns.Append(",");
                    sbReturningColumns.Append("INSERTED." + this.GetQuotedColumnName(fieldDef.FieldName));
                }

                if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
                    continue;

                var value = entry.Value;

                if (sbColumnNames.Length > 0)
                    sbColumnNames.Append(",");
                if (sbColumnValues.Length > 0)
                    sbColumnValues.Append(",");

                try
                {
                    sbColumnNames.Append(this.GetQuotedColumnName(fieldDef.FieldName));
                    sbColumnValues.Append(this.GetInsertParam(dbCmd, value, fieldDef));
                }
                catch (Exception ex)
                {
                    Log.Error("ERROR in PrepareInsertRowStatement(): " + ex.Message, ex);
                    throw;
                }
            }

            var strReturning = StringBuilderCacheAlt.ReturnAndFree(sbReturningColumns);
            strReturning = strReturning.Length > 0 ? "OUTPUT " + strReturning + " " : string.Empty;
            dbCmd.CommandText = sbColumnNames.Length > 0
                ? $"INSERT INTO {this.GetQuotedTableName(modelDef)} ({StringBuilderCache.ReturnAndFree(sbColumnNames)}) {strReturning}" +
                  $"VALUES ({StringBuilderCacheAlt.ReturnAndFree(sbColumnValues)})"
                : $"INSERT INTO {this.GetQuotedTableName(modelDef)} {strReturning}DEFAULT VALUES";
        }

        public override string ToSelectStatement(
            QueryType queryType,
            ModelDefinition modelDef,
            string selectExpression,
            string bodyExpression,
            string orderByExpression = null,
            int? offset = null,
            int? rows = null)
        {
            var sb = StringBuilderCache.Allocate().Append(selectExpression).Append(bodyExpression);

            if (!offset.HasValue && !rows.HasValue || (queryType != QueryType.Select && rows != 1))
                return StringBuilderCache.ReturnAndFree(sb) + orderByExpression;

            if (offset is < 0)
                throw new ArgumentException($"Skip value:'{offset.Value}' must be>=0");

            if (rows is < 0)
                throw new ArgumentException($"Rows value:'{rows.Value}' must be>=0");

            var skip = offset ?? 0;
            var take = rows ?? int.MaxValue;

            var selectType = selectExpression.StartsWithIgnoreCase("SELECT DISTINCT") ? "SELECT DISTINCT" : "SELECT";

            //avoid Windowing function if unnecessary
            if (skip == 0)
            {
                var sql = StringBuilderCache.ReturnAndFree(sb) + orderByExpression;

                return SqlTop(sql, take, selectType);
            }

            // Required because ordering is done by Windowing function
            if (string.IsNullOrEmpty(orderByExpression))
            {
                if (modelDef.PrimaryKey == null)
                    throw new ApplicationException("Malformed model, no PrimaryKey defined");

                orderByExpression = $"ORDER BY {this.GetQuotedColumnName(modelDef, modelDef.PrimaryKey)}";
            }

            var row = take == int.MaxValue ? take : skip + take;

            var ret =
                $"SELECT * FROM (SELECT {selectExpression.Substring(selectType.Length)}, ROW_NUMBER() OVER ({orderByExpression}) As RowNum {bodyExpression}) AS RowConstrainedResult WHERE RowNum > {skip} AND RowNum <= {row}";

            return ret;
        }

        protected static string SqlTop(string sql, int take, string selectType = null)
        {
            selectType ??= sql.StartsWithIgnoreCase("SELECT DISTINCT") ? "SELECT DISTINCT" : "SELECT";

            if (take == int.MaxValue)
                return sql;

            if (sql.Length < "SELECT".Length)
                return sql;

            return selectType + " TOP " + take + sql.Substring(selectType.Length);
        }

        // SELECT without RowNum and prefer aliases to be able to use in SELECT IN () Reference Queries
        public static string UseAliasesOrStripTablePrefixes(string selectExpression)
        {
            if (selectExpression.IndexOf('.') < 0)
                return selectExpression;

            var sb = StringBuilderCache.Allocate();
            var selectToken = selectExpression.SplitOnFirst(' ');
            var tokens = selectToken[1].Split(',');
            foreach (var token in tokens)
            {
                if (sb.Length > 0)
                    sb.Append(", ");

                var field = token.Trim();

                var aliasParts = field.SplitOnLast(' ');
                if (aliasParts.Length > 1)
                {
                    sb.Append(" " + aliasParts[aliasParts.Length - 1]);
                    continue;
                }

                var parts = field.SplitOnLast('.');
                if (parts.Length > 1)
                {
                    sb.Append(" " + parts[parts.Length - 1]);
                }
                else
                {
                    sb.Append(" " + field);
                }
            }

            var sqlSelect = selectToken[0] + " " + StringBuilderCache.ReturnAndFree(sb).Trim();
            return sqlSelect;
        }

        public override string GetLoadChildrenSubSelect<From>(SqlExpression<From> expr)
        {
            if (!expr.OrderByExpression.IsNullOrEmpty() && expr.Rows == null)
            {
                var modelDef = expr.ModelDef;
                expr.Select(this.GetQuotedColumnName(modelDef, modelDef.PrimaryKey)).ClearLimits()
                    .OrderBy(string.Empty); // Invalid in Sub Selects

                var subSql = expr.ToSelectStatement();

                return subSql;
            }

            return base.GetLoadChildrenSubSelect(expr);
        }

        public override string SqlCurrency(string fieldOrValue, string currencySymbol) =>
            this.SqlConcat(
                new[] { "'" + currencySymbol + "'", $"CONVERT(VARCHAR, CONVERT(MONEY, {fieldOrValue}), 1)" });

        public override string SqlBool(bool value) => value ? "1" : "0";

        public override string SqlLimit(int? offset = null, int? rows = null) =>
            rows == null && offset == null ? string.Empty :
            rows != null ? "OFFSET " + offset.GetValueOrDefault() + " ROWS FETCH NEXT " + rows + " ROWS ONLY" :
            "OFFSET " + offset.GetValueOrDefault(int.MaxValue) + " ROWS";

        public override string SqlCast(object fieldOrValue, string castAs) =>
            castAs == Sql.VARCHAR ? $"CAST({fieldOrValue} AS VARCHAR(MAX))" : $"CAST({fieldOrValue} AS {castAs})";

        public override string SqlRandom => "NEWID()";

        public override void EnableForeignKeysCheck(IDbCommand cmd) =>
            cmd.ExecNonQuery("EXEC sp_msforeachtable \"ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all\"");

        public override Task EnableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default) =>
            cmd.ExecNonQueryAsync(
                "EXEC sp_msforeachtable \"ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all\"",
                null,
                token);

        public override void DisableForeignKeysCheck(IDbCommand cmd) =>
            cmd.ExecNonQuery("EXEC sp_msforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"");

        public override Task DisableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default) =>
            cmd.ExecNonQueryAsync("EXEC sp_msforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"", null, token);

        protected SqlConnection Unwrap(IDbConnection db) => (SqlConnection)db.ToDbConnection();

        protected SqlCommand Unwrap(IDbCommand cmd) => (SqlCommand)cmd.ToDbCommand();

        protected SqlDataReader Unwrap(IDataReader reader) => (SqlDataReader)reader;

#if ASYNC
        public override Task OpenAsync(IDbConnection db, CancellationToken token = default) =>
            this.Unwrap(db).OpenAsync(token);

        public override Task<IDataReader> ExecuteReaderAsync(IDbCommand cmd, CancellationToken token = default) =>
            this.Unwrap(cmd).ExecuteReaderAsync(token).Then(x => (IDataReader)x);

        public override Task<int> ExecuteNonQueryAsync(IDbCommand cmd, CancellationToken token = default) =>
            this.Unwrap(cmd).ExecuteNonQueryAsync(token);

        public override Task<object> ExecuteScalarAsync(IDbCommand cmd, CancellationToken token = default) =>
            this.Unwrap(cmd).ExecuteScalarAsync(token);

        public override Task<bool> ReadAsync(IDataReader reader, CancellationToken token = default) =>
            this.Unwrap(reader).ReadAsync(token);

        public override async Task<List<T>> ReaderEach<T>(
            IDataReader reader,
            Func<T> fn,
            CancellationToken token = default)
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

        public override async Task<Return> ReaderEach<Return>(
            IDataReader reader,
            Action fn,
            Return source,
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

        public override async Task<T> ReaderRead<T>(IDataReader reader, Func<T> fn, CancellationToken token = default)
        {
            try
            {
                if (await this.ReadAsync(reader, token).ConfigureAwait(false))
                    return fn();

                return default(T);
            }
            finally
            {
                reader.Dispose();
            }
        }

#endif

        public override string GetUtcDateFunction()
        {
            return "GETUTCDATE()";
        }

        public override string DateDiffFunction(string interval, string date1, string date2)
        {
            return $"DATEDIFF({interval}, {date1}, {date2})";
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
            return $"ISNULL(({expression}), {alternateValue})";
        }

        public override string ConvertFlag(string expression)
        {
            return $"CONVERT([bit], sign({expression}))";
        }

        public override string DatabaseFragmentationInfo(string database)
        {
            var sb = new StringBuilder();

            sb.AppendLine("SELECT object_name(IPS.object_id) AS [TableName],");
            sb.AppendLine("IPS.avg_fragmentation_in_percent,");
            sb.AppendLine("IPS.avg_fragment_size_in_pages,");
            sb.AppendLine("IPS.avg_page_space_used_in_percent,");
            sb.AppendLine("IPS.record_count,");
            sb.AppendLine("IPS.ghost_record_count,");
            sb.AppendLine("IPS.fragment_count,");
            sb.AppendLine("IPS.avg_fragment_size_in_pages");
            sb.AppendLine($"FROM sys.dm_db_index_physical_stats(db_id(N'{database}'), NULL, NULL, NULL, 'DETAILED') IPS");
            sb.AppendLine("JOIN sys.tables ST WITH(nolock) ON IPS.object_id = ST.object_id");
            sb.AppendLine("JOIN sys.indexes SI WITH(nolock) ON IPS.object_id = SI.object_id AND IPS.index_id = SI.index_id");
            sb.AppendLine("WHERE ST.is_ms_shipped = 0");
            sb.AppendLine("ORDER BY avg_fragmentation_in_percent desc");

            return sb.ToString();
        }

        public override string DatabaseSize(string database)
        {
            return "SELECT sum(reserved_page_count) * 8.0 / 1024 FROM sys.dm_db_partition_stats";
        }

        public override string SQLVersion()
        {
            return "select @@version";
        }

        public override string SQLServerName()
        {
            return "Microsoft SQL Server";
        }

        public override string ShrinkDatabase(string database)
        {
            return $"DBCC SHRINKDATABASE(N'{database}')";
        }

        public override string ReIndexDatabase(string database, string objectQualifier)
        {
            var sb = new StringBuilder();

            sb.AppendLine("DECLARE @MyTable VARCHAR(255)");
            sb.AppendLine("DECLARE myCursor");
            sb.AppendLine("CURSOR FOR");
            sb.AppendFormat(
                "SELECT table_name FROM information_schema.tables WHERE table_type = 'base table' AND table_name LIKE '{0}%'",
                objectQualifier);
            sb.AppendLine("OPEN myCursor");
            sb.AppendLine("FETCH NEXT");
            sb.AppendLine("FROM myCursor INTO @MyTable");
            sb.AppendLine("WHILE @@FETCH_STATUS = 0");
            sb.AppendLine("BEGIN");
            sb.AppendLine("PRINT 'Reindexing Table:  ' + @MyTable");
            sb.AppendLine("DBCC DBREINDEX(@MyTable, '', 80)");
            sb.AppendLine("FETCH NEXT");
            sb.AppendLine("FROM myCursor INTO @MyTable");
            sb.AppendLine("END");
            sb.AppendLine("CLOSE myCursor");
            sb.AppendLine("DEALLOCATE myCursor");

            return sb.ToString();
        }

        public override string ChangeRecoveryMode(string database, string mode)
        {
            return $"ALTER DATABASE {database} SET RECOVERY {mode}";
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
            var sqlCommand = command as SqlCommand;

            SqlDataReader reader = null;
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
