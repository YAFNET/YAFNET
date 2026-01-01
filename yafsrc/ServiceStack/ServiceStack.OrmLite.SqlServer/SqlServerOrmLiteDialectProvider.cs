// ***********************************************************************
// <copyright file="SqlServerOrmLiteDialectProvider.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

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
using ServiceStack.OrmLite.Base.Text;
using ServiceStack.OrmLite.SqlServer.Converters;


namespace ServiceStack.OrmLite.SqlServer
{
    using System.Data.Common;
    using System.Text;

    /// <summary>
    /// Class SqlServerOrmLiteDialectProvider.
    /// Implements the <see cref="SqlServerOrmLiteDialectProvider" />
    /// </summary>
    /// <seealso cref="SqlServerOrmLiteDialectProvider" />
    public class SqlServerOrmLiteDialectProvider : OrmLiteDialectProviderBase<SqlServerOrmLiteDialectProvider>
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static SqlServerOrmLiteDialectProvider Instance = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerOrmLiteDialectProvider" /> class.
        /// </summary>
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
                { OrmLiteVariables.False, this.SqlBool(false) }
            };
        }

        /// <summary>
        /// Quote the string so that it can be used inside an SQL-expression
        /// Escape quotes inside the string
        /// </summary>
        /// <param name="paramValue">The parameter value.</param>
        /// <returns>System.String.</returns>
        public override string GetQuotedValue(string paramValue)
        {
            return (this.StringConverter.UseUnicode ? "N'" : "'") + paramValue.Replace("'", "''") + "'";
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="options">The options.</param>
        /// <returns>IDbConnection.</returns>
        public override IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options)
        {
            var isFullConnectionString = connectionString.Contains(';');

            if (!isFullConnectionString)
            {
                var filePath = connectionString;

                var filePathWithExt = filePath.EndsWithIgnoreCase(".mdf") ? filePath : filePath + ".mdf";

                var fileName = Path.GetFileName(filePathWithExt);
                var dbName = fileName[..^".mdf".Length];

                connectionString =
                    $@"Data Source=.\SQLEXPRESS;AttachDbFilename={filePathWithExt};Initial Catalog={dbName};Integrated Security=True;User Instance=True;";
            }

            if (options == null)
            {
                return new SqlConnection(connectionString);
            }

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

            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// SQLs the expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>SqlExpression&lt;T&gt;.</returns>
        public override SqlExpression<T> SqlExpression<T>()
        {
            return new SqlServerExpression<T>(this);
        }

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <returns>IDbDataParameter.</returns>
        public override IDbDataParameter CreateParam()
        {
            return new SqlParameter();
        }

        /// <summary>
        /// The default schema
        /// </summary>
        private const string DefaultSchema = "dbo";

        /// <summary>
        /// Converts to tablenamesstatement.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns>System.String.</returns>
        public override string ToTableNamesStatement(string schema)
        {
            var sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";
            return sql + " AND TABLE_SCHEMA = {0}".SqlFmt(this, schema ?? DefaultSchema);
        }

        /// <summary>
        /// Return table, row count SQL for listing all tables with their row counts
        /// </summary>
        /// <param name="live">If true returns live current row counts of each table (slower), otherwise returns cached row counts from RDBMS table stats</param>
        /// <param name="schema">The table schema if any</param>
        /// <returns>System.String.</returns>
        public override string ToTableNamesWithRowCountsStatement(bool live, string schema)
        {
            var schemaSql = " AND s.Name = {0}".SqlFmt(this, schema ?? DefaultSchema);

            var sql = """
                      SELECT t.NAME, p.rows FROM sys.tables t INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                                                     INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
                                                     INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
                                               WHERE t.is_ms_shipped = 0
                      """ + schemaSql + " GROUP BY t.NAME, p.Rows";
            return sql;
        }

        /// <summary>
        /// Gets the schemas.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public override List<string> GetSchemas(IDbCommand dbCmd)
        {
            var sql = "SELECT DISTINCT TABLE_SCHEMA FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";
            return dbCmd.SqlColumn<string>(sql);
        }

        /// <summary>
        /// Gets the schema tables.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <returns>Dictionary&lt;System.String, List&lt;System.String&gt;&gt;.</returns>
        public override Dictionary<string, List<string>> GetSchemaTables(IDbCommand dbCmd)
        {
            var sql = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";
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
            var sql = $"SELECT count(*) FROM sys.schemas WHERE name = '{schemaName.SqlParam()}'";
            var result = dbCmd.ExecLongScalar(sql);
            return result > 0;
        }

        /// <summary>
        /// Does schema exist as an asynchronous operation.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        public async override Task<bool> DoesSchemaExistAsync(
            IDbCommand dbCmd,
            string schemaName,
            CancellationToken token = default)
        {
            var sql = $"SELECT count(*) FROM sys.schemas WHERE name = '{schemaName.SqlParam()}'";
            var result = await dbCmd.ExecLongScalarAsync(sql, token);
            return result > 0;
        }

        /// <summary>
        /// Converts to createschemastatement.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        public override string ToCreateSchemaStatement(string schemaName)
        {
            var sql = $"CREATE SCHEMA [{NamingStrategy.GetSchemaName(schemaName)}]";
            return sql;
        }

        /// <summary>
        /// Converts to createsavepoint.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public override string ToCreateSavePoint(string name)
        {
            return $"SAVE TRANSACTION {name}";
        }

        /// <summary>
        /// Converts to releasesavepoint.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public override string ToReleaseSavePoint(string name)
        {
            return null;
        }

        /// <summary>
        /// Converts to rollbacksavepoint.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public override string ToRollbackSavePoint(string name)
        {
            return $"ROLLBACK TRANSACTION {name}";
        }

        /// <summary>
        /// Doeses the table exist.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="tableRef">The table reference.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool DoesTableExist(IDbCommand dbCmd, TableRef tableRef)
        {
            var tableName = GetTableNameOnly(tableRef);
            var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}".SqlFmt(this, tableName);

            var schema = GetSchemaName(tableRef);
            if (schema != null)
            {
                sql += " AND TABLE_SCHEMA = {0}".SqlFmt(this, schema);
            }
            else
            {
                sql += " AND TABLE_SCHEMA <> 'Security'";
            }

            var result = dbCmd.ExecLongScalar(sql);

            return result > 0;
        }

        /// <summary>
        /// Does table exist as an asynchronous operation.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="tableRef">The table reference.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        public async override Task<bool> DoesTableExistAsync(
            IDbCommand dbCmd,
            TableRef tableRef,
            CancellationToken token = default)
        {
            var tableName = GetTableNameOnly(tableRef);
            var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}".SqlFmt(this, tableName);

            var schema = GetSchemaName(tableRef);
            if (schema != null)
            {
                sql += " AND TABLE_SCHEMA = {0}".SqlFmt(this, schema);
            }
            else
            {
                sql += " AND TABLE_SCHEMA <> 'Security'";
            }

            var result = await dbCmd.ExecLongScalarAsync(sql, token);

            return result > 0;
        }

        /// <summary>
        /// Doeses the column exist.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="tableRef">The table reference.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool DoesColumnExist(
            IDbConnection db,
            string columnName,
            TableRef tableRef)
        {
            var tableName = GetTableNameOnly(tableRef);
            var sql =
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName"
                    .SqlFmt(this, tableName, columnName);

            var schema = GetSchemaName(tableRef);
            if (schema != null)
            {
                sql += " AND TABLE_SCHEMA = @schema";
            }

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
        public async override Task<bool> DoesColumnExistAsync(
            IDbConnection db,
            string columnName,
            TableRef tableRef,
            CancellationToken token = default)
        {
            var tableName = GetTableNameOnly(tableRef);
            var sql =
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName"
                    .SqlFmt(this, tableName, columnName);

            var schema = GetSchemaName(tableRef);
            if (schema != null)
            {
                sql += " AND TABLE_SCHEMA = @schema";
            }

            var result = await db.SqlScalarAsync<long>(sql, new { tableName, columnName, schema }, token: token);

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
            var tableName = this.UnquotedTable(tableRef);
            var sql =
                "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName"
                    .SqlFmt(this, tableName, columnName);

            var schema = GetSchemaName(tableRef);
            if (schema != null)
            {
                sql += " AND TABLE_SCHEMA = @schema";
            }

            return db.SqlScalar<string>(sql, new { tableName, columnName, schema });
        }

        /// <summary>
        /// Columns the is nullable.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="tableRef">The table reference.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool ColumnIsNullable(
            IDbConnection db,
            string columnName,
            TableRef tableRef)
        {
            var tableName = this.UnquotedTable(tableRef);
            var sql =
                "SELECT is_nullable FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName"
                    .SqlFmt(this, tableName, columnName);

            var schema = this.GetSchemaName(tableRef);
            if (schema != null)
            {
                sql += " AND TABLE_SCHEMA = @schema";
            }

            return db.SqlScalar<string>(sql, new { tableName, columnName, schema }) == "YES";
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
        /// Gets the foreign key on delete clause.
        /// </summary>
        /// <param name="foreignKey">The foreign key.</param>
        /// <returns>System.String.</returns>
        public override string GetForeignKeyOnDeleteClause(ForeignKeyConstraint foreignKey)
        {
            return "RESTRICT" == (foreignKey.OnDelete ?? string.Empty).ToUpper()
                ? string.Empty
                : base.GetForeignKeyOnDeleteClause(foreignKey);
        }

        /// <summary>
        /// Gets the foreign key on update clause.
        /// </summary>
        /// <param name="foreignKey">The foreign key.</param>
        /// <returns>System.String.</returns>
        public override string GetForeignKeyOnUpdateClause(ForeignKeyConstraint foreignKey)
        {
            return (foreignKey.OnUpdate ?? string.Empty).ToUpper() == "RESTRICT"
                ? string.Empty
                : base.GetForeignKeyOnUpdateClause(foreignKey);
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

            sb.Append('\'');

            sb.Append(" end");

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

        /// <summary>
        /// Gets the create index view.
        /// </summary>
        /// <param name="modelDef">The model definition.</param>
        /// <param name="name">The name.</param>
        /// <param name="selectSql">The select SQL.</param>
        /// <returns>System.String.</returns>
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

        /// <summary>
        /// Gets the drop index view.
        /// </summary>
        /// <param name="modelDef">The model definition.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
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

        /// <summary>
        /// Gets the drop index constraint.
        /// </summary>
        /// <param name="modelDef">The model definition.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
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

        /// <summary>
        /// Gets the add composite primary key sql command.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="modelDef">The model definition.</param>
        /// <param name="fieldNameA">The field name a.</param>
        /// <param name="fieldNameB">The field name b.</param>
        /// <returns>Returns the SQL Command</returns>
        public override string GetAddCompositePrimaryKey(string database, ModelDefinition modelDef, string fieldNameA, string fieldNameB)
        {
            var sb = StringBuilderCache.Allocate();

            sb.Append("alter table ");
            sb.AppendFormat(
                "{0} with nocheck add constraint [PK_{0}] primary key clustered ({1},{2})",
                this.NamingStrategy.GetTableName(modelDef),
                fieldNameA,
                fieldNameB);

            return StringBuilderCache.ReturnAndFree(sb);
        }

        /// <summary>
        /// Gets the name of the primary key.
        /// </summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns>Returns the Primary Key Name</returns>
        public override string GetPrimaryKeyName(ModelDefinition modelDef)
        {
            var sb = StringBuilderCache.Allocate();

            sb.Append("SELECT name FROM sys.indexes WHERE ");
            sb.AppendFormat(
                "object_id = OBJECT_ID(N'[{0}].[{1}]')",
                this.NamingStrategy.GetSchemaName(modelDef),
                this.NamingStrategy.GetTableName(modelDef));
            sb.Append("and is_primary_key = 1");

            return StringBuilderCache.ReturnAndFree(sb);
        }

        /// <summary>
        /// Gets the drop primary key constraint.
        /// </summary>
        /// <param name="database">the database name</param>
        /// <param name="modelDef">The model definition.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public override string GetDropPrimaryKeyConstraint(string database, ModelDefinition modelDef, string name)
        {
            var sb = StringBuilderCache.Allocate();

            string foreignKeyName;

            if (name.IsNullOrEmpty())
            {
                foreignKeyName = $"PK_{this.NamingStrategy.GetTableName(modelDef)}";
            }
            else
            {
                foreignKeyName = name.StartsWith("PK_", StringComparison.CurrentCultureIgnoreCase)
                    ? name
                    : $"PK_{this.NamingStrategy.GetTableName(modelDef)}_{name}";
            }

            var tableName = this.GetQuotedTableName(modelDef);

            sb.Append("IF EXISTS (");
            sb.Append("SELECT top 1 1 FROM sys.indexes WHERE ");
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


        /// <summary>
        /// Gets the drop primary key constraint.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="modelDef">The model definition.</param>
        /// <param name="name">The name.</param>
        /// <param name="fieldNameA">The field name a.</param>
        /// <param name="fieldNameB">The field name b.</param>
        /// <returns>System.String.</returns>
        public override string GetDropPrimaryKeyConstraint(string database, ModelDefinition modelDef, string name, string fieldNameA, string fieldNameB)
        {
            var sb = StringBuilderCache.Allocate();

            var foreignKeyName = name.IsNullOrEmpty() ? $"PK_{this.NamingStrategy.GetTableName(modelDef)}" :
                                 name.StartsWith("PK_", StringComparison.CurrentCultureIgnoreCase) ? name :
                                 $"PK_{this.NamingStrategy.GetTableName(modelDef)}_{name}";

            var tableName = this.GetQuotedTableName(modelDef);

            sb.Append("IF EXISTS (");
            sb.Append("SELECT top 1 1 FROM sys.indexes WHERE ");
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

        /// <summary>
        /// Gets the drop foreign key constraint.
        /// </summary>
        /// <param name="modelDef">The model definition.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public override string GetDropForeignKeyConstraint(ModelDefinition modelDef, string name)
        {
            var sb = StringBuilderCache.Allocate();

            var foreignKeyName = $"FK_{this.NamingStrategy.GetTableName(modelDef)}_{name}";

            var tableName = this.GetQuotedTableName(modelDef);

            sb.Append("IF EXISTS (");
            sb.Append("SELECT top 1 1 FROM sys.foreign_keys WHERE ");
            sb.AppendFormat(
                "parent_object_id = OBJECT_ID(N'[{0}].[{1}]')",
                this.NamingStrategy.GetSchemaName(modelDef),
                this.NamingStrategy.GetTableName(modelDef));
            sb.AppendFormat("and name = N'{0}')", foreignKeyName);
            sb.Append(" BEGIN");
            sb.AppendFormat("  ALTER TABLE {1} DROP constraint {0}", foreignKeyName, tableName);
            sb.Append(" END");

            return StringBuilderCache.ReturnAndFree(sb);
        }

        /// <summary>
        /// Gets the name of the constraint.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="modelDef">The model definition.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>string.</returns>
        public override string GetConstraintName(string database, ModelDefinition modelDef, string fieldName)
        {
            var sb = StringBuilderCache.Allocate();

            sb.Append("SELECT NAME FROM ");
            sb.Append("sys.default_constraints WHERE ");
            sb.AppendFormat(
                "parent_object_id = OBJECT_ID(N'[{0}].[{1}]') and col_name(parent_object_id, parent_column_id) = '{2}'",
                this.NamingStrategy.GetSchemaName(modelDef),
                this.NamingStrategy.GetTableName(modelDef),
                fieldName);

            return StringBuilderCache.ReturnAndFree(sb);
        }

        /// <summary>
        /// Gets the drop constraint.
        /// </summary>
        /// <param name="modelDef">The model definition.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public override string GetDropConstraint(ModelDefinition modelDef, string name)
        {
            var sb = StringBuilderCache.Allocate();

            var tableName = this.GetQuotedTableName(modelDef);

            sb.Append("IF ");
            sb.AppendFormat(
                "OBJECTPROPERTY(OBJECT_ID(N'{0}'), 'IsConstraint') = 1",
                name);
            sb.Append("BEGIN");
            sb.AppendFormat("  ALTER TABLE {1} DROP constraint {0}", name, tableName);
            sb.Append(" END");

            return StringBuilderCache.ReturnAndFree(sb);
        }

        /// <summary>
        /// Gets the drop foreign key constraints.
        /// </summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns>System.String.</returns>
        public override string GetDropForeignKeyConstraints(ModelDefinition modelDef)
        {
            // TODO: find out if this should go in base class?
            var sb = StringBuilderCache.Allocate();

            modelDef.FieldDefinitions.Where(fieldDef => fieldDef.ForeignKey != null).ToList().ForEach(
                fieldDef =>
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
                });

            return StringBuilderCache.ReturnAndFree(sb);
        }

        /// <summary>
        /// Converts to alter column statement.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.String.</returns>
        public override string ToAddColumnStatement(TableRef tableRef, FieldDefinition fieldDef)
        {
            return $"ALTER TABLE {this.QuoteTable(tableRef)} ADD {this.GetColumnDefinition(fieldDef)};";
        }

        /// <summary>
        /// Converts to altercolumnstatement.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.String.</returns>
        public override string ToAlterColumnStatement(TableRef tableRef, FieldDefinition fieldDef)
        {
            return
                $"ALTER TABLE {this.QuoteTable(tableRef)} ALTER COLUMN {this.GetColumnDefinition(fieldDef)};";
        }

        /// <summary>
        /// Converts to changecolumnnamestatement.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <param name="oldColumn">The old column.</param>
        /// <returns>System.String.</returns>
        public override string ToChangeColumnNameStatement(TableRef tableRef, FieldDefinition fieldDef, string oldColumn)
        {
            var objectName = $"{this.QuoteTable(tableRef)}.{this.GetQuotedColumnName(oldColumn)}";

            return
                $"EXEC sp_rename {this.GetQuotedValue(objectName)}, {this.GetQuotedValue(fieldDef.FieldName)}, {this.GetQuotedValue("COLUMN")};";
        }

        /// <summary>
        /// Converts to renamecolumnstatement.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <param name="oldColumn">The old column.</param>
        /// <param name="newColumn">The new column.</param>
        /// <returns>System.String.</returns>
        public override string ToRenameColumnStatement(TableRef tableRef, string oldColumn, string newColumn)
        {
            var objectName = $"{this.QuoteTable(tableRef)}.{this.GetQuotedColumnName(oldColumn)}";
            return $"EXEC sp_rename {this.GetQuotedValue(objectName)}, {this.GetQuotedColumnName(newColumn)}, 'COLUMN';";
        }

        public override string ToDropIndexStatement<T>(string indexName)
        {
            return $"DROP INDEX IF EXISTS {this.GetQuotedName(indexName)} ON {this.GetQuotedTableName(typeof(T))};";
        }

        /// <summary>
        /// Gets the automatic increment definition.
        /// </summary>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetAutoIncrementDefinition(FieldDefinition fieldDef)
        {
            return this.AutoIncrementDefinition;
        }

        /// <summary>
        /// Gets the automatic identifier default value.
        /// </summary>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.String.</returns>
        public override string GetAutoIdDefaultValue(FieldDefinition fieldDef)
        {
            return fieldDef.FieldType == typeof(Guid) ? "newid()" : null;
        }

        /// <summary>
        /// Gets the column definition.
        /// </summary>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.String.</returns>
        public override string GetColumnDefinition(FieldDefinition fieldDef)
        {
            // https://msdn.microsoft.com/en-us/library/ms182776.aspx
            if (fieldDef.IsRowVersion)
            {
                return $"{fieldDef.FieldName} rowversion NOT NULL";
            }

            var fieldDefinition = this.ResolveFragment(fieldDef.CustomFieldDefinition) ?? this.GetColumnTypeDefinition(
                fieldDef.ColumnType,
                fieldDef.FieldLength,
                fieldDef.Scale);

            var sql = StringBuilderCache.Allocate();
            sql.Append($"{this.GetQuotedColumnName(fieldDef)} {fieldDefinition}");

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
                {
                    sql.Append(" NONCLUSTERED");
                }

                if (fieldDef.AutoIncrement)
                {
                    sql.Append(' ').Append(this.GetAutoIncrementDefinition(fieldDef));
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
                if (fieldDef.DefaultValueConstraint != null)
                {
                    sql.Append(" CONSTRAINT ").Append(this.GetQuotedName(fieldDef.DefaultValueConstraint));
                }
                sql.AppendFormat(this.DefaultValueFormat, defaultValue);
            }

            return StringBuilderCache.ReturnAndFree(sql);
        }

        /// <summary>
        /// Gets the column definition.
        /// </summary>
        /// <param name="fieldDef">The field definition.</param>
        /// <param name="modelDef">The model definition.</param>
        /// <returns>System.String.</returns>
        public override string GetColumnDefinition(FieldDefinition fieldDef, ModelDefinition modelDef)
        {
            // https://msdn.microsoft.com/en-us/library/ms182776.aspx
            if (fieldDef.IsRowVersion)
            {
                return $"{fieldDef.FieldName} rowversion NOT NULL";
            }

            var fieldDefinition = this.ResolveFragment(fieldDef.CustomFieldDefinition) ?? this.GetColumnTypeDefinition(
                fieldDef.ColumnType,
                fieldDef.FieldLength,
                fieldDef.Scale);

            var sql = StringBuilderCache.Allocate();
            sql.Append($"{this.GetQuotedColumnName(fieldDef)} {fieldDefinition}");

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
                {
                    sql.Append(" NONCLUSTERED");
                }

                if (fieldDef.AutoIncrement)
                {
                    sql.Append(' ').Append(this.GetAutoIncrementDefinition(fieldDef));
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

        /// <summary>
        /// Converts to dropconstraintstatement.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <param name="constraintName">Name of the constraint.</param>
        /// <returns>string.</returns>
        public override string ToDropConstraintStatement(TableRef tableRef, string constraintName)
        {
            return $"ALTER TABLE {this.QuoteTable(tableRef)} DROP CONSTRAINT {this.GetQuotedName(constraintName)};";
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
            config ??= new BulkInsertConfig();
            if (config.Mode == BulkInsertMode.Sql)
            {
                base.BulkInsert(db, objs, config);
                return;
            }

            var sqlConn = (SqlConnection)db.ToDbConnection();
            using var bulkCopy = new SqlBulkCopy(sqlConn);
            var modelDef = ModelDefinition<T>.Definition;

            bulkCopy.BatchSize = config.BatchSize;
            bulkCopy.DestinationTableName = modelDef.ModelName;

            var table = new DataTable();
            var fieldDefs = this.GetInsertFieldDefinitions(modelDef, insertFields: config.InsertFields);
            foreach (var fieldDef in fieldDefs)
            {
                if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
                {
                    continue;
                }

                var columnName = this.NamingStrategy.GetColumnName(fieldDef.FieldName);
                bulkCopy.ColumnMappings.Add(columnName, columnName);

                var converter = this.GetConverterBestMatch(fieldDef);
                var colType = converter.DbType switch
                {
                    DbType.String => typeof(string),
                    DbType.Int32 => typeof(int),
                    DbType.Int64 => typeof(long),
                    _ => Nullable.GetUnderlyingType(fieldDef.FieldType) ?? fieldDef.FieldType
                };

                table.Columns.Add(columnName, colType);
            }

            foreach (var obj in objs)
            {
                var row = table.NewRow();
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
                    var dbValue = converter.ToDbValue(fieldDef.FieldType, value);
                    var columnName = this.NamingStrategy.GetColumnName(fieldDef.FieldName);
                    dbValue ??= DBNull.Value;
                    row[columnName] = dbValue;
                }

                table.Rows.Add(row);
            }

            bulkCopy.WriteToServer(table);
        }

        /// <summary>
        /// Converts to insertrowstatement.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <param name="insertFields">The insert fields.</param>
        /// <returns>System.String.</returns>
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
                    {
                        sbReturningColumns.Append(',');
                    }

                    sbReturningColumns.Append("INSERTED." + this.GetQuotedColumnName(fieldDef));
                }

                if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
                {
                    continue;
                }

                if (sbColumnNames.Length > 0)
                {
                    sbColumnNames.Append(',');
                }

                if (sbColumnValues.Length > 0)
                {
                    sbColumnValues.Append(',');
                }

                try
                {
                    sbColumnNames.Append(this.GetQuotedColumnName(fieldDef));
                    sbColumnValues.Append(this.GetParam(this.SanitizeFieldNameForParamName(fieldDef.FieldName)));

                    this.AddParameter(cmd, fieldDef);
                }
                catch (Exception ex)
                {
                    Log.Error("ERROR in ToInsertRowStatement(): " + ex.Message, ex);
                    throw;
                }
            }

            foreach (var fieldDef in modelDef.AutoIdFields.Where(fieldDef => !fieldDefs.Contains(fieldDef)))
            {
                if (sbReturningColumns.Length > 0)
                {
                    sbReturningColumns.Append(',');
                }

                sbReturningColumns.Append("INSERTED." + this.GetQuotedColumnName(fieldDef));
            }

            var strReturning = StringBuilderCacheAlt.ReturnAndFree(sbReturningColumns);
            strReturning = strReturning.Length > 0 ? "OUTPUT " + strReturning + " " : string.Empty;
            var sql = sbColumnNames.Length > 0
                ? $"INSERT INTO {this.GetQuotedTableName(modelDef)} ({StringBuilderCache.ReturnAndFree(sbColumnNames)}) " +
                  strReturning + $"VALUES ({StringBuilderCacheAlt.ReturnAndFree(sbColumnValues)})"
                : $"INSERT INTO {this.GetQuotedTableName(modelDef)} {strReturning} DEFAULT VALUES";

            return sql;
        }

        /// <summary>
        /// Sequences the specified schema.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="sequence">The sequence.</param>
        /// <returns>System.String.</returns>
        protected string Sequence(string schema, string sequence)
        {
            /*if (schema == null)
                return this.GetQuotedName(sequence);*/

            var escapedSchema = this.NamingStrategy.GetSchemaName(schema).Replace(".", "\".\"");

            return this.GetQuotedName(escapedSchema) + "." + this.GetQuotedName(sequence);
        }

        /// <summary>
        /// Shoulds the skip insert.
        /// </summary>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        override protected bool ShouldSkipInsert(FieldDefinition fieldDef)
        {
            return fieldDef.ShouldSkipInsert() || fieldDef.AutoId;
        }

        /// <summary>
        /// Shoulds the return on insert.
        /// </summary>
        /// <param name="modelDef">The model definition.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual bool ShouldReturnOnInsert(ModelDefinition modelDef, FieldDefinition fieldDef)
        {
            return fieldDef.ReturnOnInsert ||
                   fieldDef.IsPrimaryKey && fieldDef.AutoIncrement && this.HasInsertReturnValues(modelDef) ||
                   fieldDef.AutoId;
        }

        /// <summary>
        /// Determines whether [has insert return values] [the specified model definition].
        /// </summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns><c>true</c> if [has insert return values] [the specified model definition]; otherwise, <c>false</c>.</returns>
        public override bool HasInsertReturnValues(ModelDefinition modelDef)
        {
            return modelDef.FieldDefinitions.Exists(x => x.ReturnOnInsert || x.AutoId && x.FieldType == typeof(Guid));
        }

        /// <summary>
        /// Supportses the sequences.
        /// </summary>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual bool SupportsSequences(FieldDefinition fieldDef)
        {
            return false;
        }

        /// <summary>
        /// Enables the identity insert.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd">The command.</param>
        public override void EnableIdentityInsert<T>(IDbCommand cmd)
        {
            var tableName = cmd.GetDialectProvider().GetQuotedTableName(ModelDefinition<T>.Definition);
            cmd.ExecNonQuery($"SET IDENTITY_INSERT {tableName} ON");
        }

        /// <summary>
        /// Enables the identity insert asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd">The command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public override Task EnableIdentityInsertAsync<T>(IDbCommand cmd, CancellationToken token = default)
        {
            var tableName = cmd.GetDialectProvider().GetQuotedTableName(ModelDefinition<T>.Definition);
            return cmd.ExecNonQueryAsync($"SET IDENTITY_INSERT {tableName} ON", null, token);
        }

        /// <summary>
        /// Disables the identity insert.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd">The command.</param>
        public override void DisableIdentityInsert<T>(IDbCommand cmd)
        {
            var tableName = cmd.GetDialectProvider().GetQuotedTableName(ModelDefinition<T>.Definition);
            cmd.ExecNonQuery($"SET IDENTITY_INSERT {tableName} OFF");
        }

        /// <summary>
        /// Disables the identity insert asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd">The command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public override Task DisableIdentityInsertAsync<T>(IDbCommand cmd, CancellationToken token = default)
        {
            var tableName = cmd.GetDialectProvider().GetQuotedTableName(ModelDefinition<T>.Definition);
            return cmd.ExecNonQueryAsync($"SET IDENTITY_INSERT {tableName} OFF", null, token);
        }

        /// <summary>
        /// Prepares the parameterized insert statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd">The command.</param>
        /// <param name="insertFields">The insert fields.</param>
        /// <param name="shouldInclude">The should include.</param>
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
                    {
                        sbReturningColumns.Append(',');
                    }

                    sbReturningColumns.Append("INSERTED." + this.GetQuotedColumnName(fieldDef));
                }

                if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId && shouldInclude?.Invoke(fieldDef) != true)
                {
                    continue;
                }

                if (sbColumnNames.Length > 0)
                {
                    sbColumnNames.Append(',');
                }

                if (sbColumnValues.Length > 0)
                {
                    sbColumnValues.Append(',');
                }

                try
                {
                    sbColumnNames.Append(this.GetQuotedColumnName(fieldDef));

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
                {
                    continue;
                }

                if (sbReturningColumns.Length > 0)
                {
                    sbReturningColumns.Append(',');
                }

                sbReturningColumns.Append("INSERTED." + this.GetQuotedColumnName(fieldDef));
            }

            var strReturning = StringBuilderCacheAlt.ReturnAndFree(sbReturningColumns);
            strReturning = strReturning.Length > 0 ? "OUTPUT " + strReturning + " " : string.Empty;
            cmd.CommandText = sbColumnNames.Length > 0
                ? $"INSERT INTO {this.GetQuotedTableName(modelDef)} ({StringBuilderCache.ReturnAndFree(sbColumnNames)}) {strReturning}" +
                  $" VALUES ({StringBuilderCacheAlt.ReturnAndFree(sbColumnValues)})"
                : $" INSERT INTO {this.GetQuotedTableName(modelDef)}{strReturning} DEFAULT VALUES";
        }

        /// <summary>
        /// Prepares the insert row statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="args">The arguments.</param>
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
                    {
                        sbReturningColumns.Append(',');
                    }

                    sbReturningColumns.Append("INSERTED." + this.GetQuotedColumnName(fieldDef));
                }

                if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
                {
                    continue;
                }

                var value = entry.Value;

                if (sbColumnNames.Length > 0)
                {
                    sbColumnNames.Append(',');
                }

                if (sbColumnValues.Length > 0)
                {
                    sbColumnValues.Append(',');
                }

                try
                {
                    sbColumnNames.Append(this.GetQuotedColumnName(fieldDef));
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
                  $" VALUES ({StringBuilderCacheAlt.ReturnAndFree(sbColumnValues)})"
                : $" INSERT INTO {this.GetQuotedTableName(modelDef)} {strReturning}DEFAULT VALUES";
        }

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
        /// <exception cref="System.ArgumentException">Skip value:'{offset.Value}' must be>=0</exception>
        /// <exception cref="System.ArgumentException">Rows value:'{rows.Value}' must be>=0</exception>
        /// <exception cref="System.ApplicationException">Malformed model, no PrimaryKey defined</exception>
        public override string ToSelectStatement(
            QueryType queryType,
            ModelDefinition modelDef,
            string selectExpression,
            string bodyExpression,
            string orderByExpression = null,
            int? offset = null,
            int? rows = null,
            ISet<string> tags = null)
        {
            var sb = StringBuilderCache.Allocate();
            this.ApplyTags(sb, tags);

            sb.Append(selectExpression)
                .Append(bodyExpression);

            if (!offset.HasValue && !rows.HasValue || queryType != QueryType.Select && rows != 1)
            {
                return StringBuilderCache.ReturnAndFree(sb) + orderByExpression;
            }

            if (offset is < 0)
            {
                throw new ArgumentException($"Skip value:'{offset.Value}' must be>=0");
            }

            if (rows is < 0)
            {
                throw new ArgumentException($"Rows value:'{rows.Value}' must be>=0");
            }

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
                {
                    throw new ArgumentNullException("Malformed model, no PrimaryKey defined");
                }

                orderByExpression = $"ORDER BY {this.GetQuotedColumnName(modelDef, modelDef.PrimaryKey)}";
            }

            var row = take == int.MaxValue ? take : skip + take;

            var ret =
                $"SELECT * FROM (SELECT {selectExpression[selectType.Length..]}, ROW_NUMBER() OVER ({orderByExpression}) As RowNum {bodyExpression}) AS RowConstrainedResult WHERE RowNum > {skip} AND RowNum <= {row}";

            return ret;
        }

        /// <summary>
        /// SQLs the top.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="take">The take.</param>
        /// <param name="selectType">Type of the select.</param>
        /// <returns>System.String.</returns>
        static protected string SqlTop(string sql, int take, string selectType = null)
        {
            selectType ??= sql.StartsWithIgnoreCase("SELECT DISTINCT") ? "SELECT DISTINCT" : "SELECT";

            if (take == int.MaxValue)
            {
                return sql;
            }

            if (sql.Length < "SELECT".Length)
            {
                return sql;
            }

            return selectType + " TOP " + take + sql[selectType.Length..];
        }

        // SELECT without RowNum and prefer aliases to be able to use in SELECT IN () Reference Queries
        /// <summary>
        /// Uses the aliases or strip table prefixes.
        /// </summary>
        /// <param name="selectExpression">The select expression.</param>
        /// <returns>System.String.</returns>
        public static string UseAliasesOrStripTablePrefixes(string selectExpression)
        {
            if (selectExpression.IndexOf('.') < 0)
            {
                return selectExpression;
            }

            var sb = StringBuilderCache.Allocate();
            var selectToken = selectExpression.SplitOnFirst(' ');
            var tokens = selectToken[1].Split(',');
            foreach (var token in tokens)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                var field = token.Trim();

                var aliasParts = field.SplitOnLast(' ');
                if (aliasParts.Length > 1)
                {
                    sb.Append(" " + aliasParts[^1]);
                    continue;
                }

                var parts = field.SplitOnLast('.');
                if (parts.Length > 1)
                {
                    sb.Append(" " + parts[^1]);
                }
                else
                {
                    sb.Append(" " + field);
                }
            }

            var sqlSelect = selectToken[0] + " " + StringBuilderCache.ReturnAndFree(sb).Trim();
            return sqlSelect;
        }

        /// <summary>
        /// Gets the load children sub select.
        /// </summary>
        /// <typeparam name="From">The type of from.</typeparam>
        /// <param name="expr">The expr.</param>
        /// <returns>System.String.</returns>
        public override string GetLoadChildrenSubSelect<From>(SqlExpression<From> expr)
        {
            if (expr.OrderByExpression.IsNullOrEmpty() || expr.Rows != null)
            {
                return base.GetLoadChildrenSubSelect(expr);
            }

            var modelDef = expr.ModelDef;
            expr.Select(this.GetQuotedColumnName(modelDef, modelDef.PrimaryKey)).ClearLimits()
                .OrderBy(string.Empty); // Invalid in Sub Selects

            var subSql = expr.ToSelectStatement();

            return subSql;

        }

        /// <summary>
        /// SQLs the currency.
        /// </summary>
        /// <param name="fieldOrValue">The field or value.</param>
        /// <param name="currencySymbol">The currency symbol.</param>
        /// <returns>System.String.</returns>
        public override string SqlCurrency(string fieldOrValue, string currencySymbol)
        {
            return this.SqlConcat(
                ["'" + currencySymbol + "'", $"CONVERT(VARCHAR, CONVERT(MONEY, {fieldOrValue}), 1)"]);
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
        /// SQLs the limit.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="rows">The rows.</param>
        /// <returns>System.String.</returns>
        public override string SqlLimit(int? offset = null, int? rows = null)
        {
            return rows == null && offset == null ? string.Empty :
                rows != null ? $"OFFSET {offset.GetValueOrDefault()} ROWS FETCH NEXT {rows} ROWS ONLY" :
                $"OFFSET {offset.GetValueOrDefault(int.MaxValue)} ROWS";
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
                ? $"CAST({fieldOrValue} AS VARCHAR(MAX))"
                : $"CAST({fieldOrValue} AS {castAs})";
        }

        /// <summary>
        /// Gets the SQL random.
        /// </summary>
        /// <value>The SQL random.</value>
        public override string SqlRandom => "NEWID()";

        // strftime('%Y-%m-%d %H:%M:%S', 'now')
        public Dictionary<string, string> DateFormatMap = new() {
            {"%Y", "YYYY"},
            {"%m", "MM"},
            {"%d", "DD"},
            {"%H", "HH"},
            {"%M", "mm"},
            {"%S", "ss"},
        };
        public override string SqlDateFormat(string quotedColumn, string format)
        {
            var fmt = format.Contains('\'')
                ? format.Replace("'", "")
                : format;
            foreach (var entry in DateFormatMap)
            {
                fmt = fmt.Replace(entry.Key, entry.Value);
            }
            return $"FORMAT({quotedColumn}, '{fmt}')";
        }

        /// <summary>
        /// Enables the foreign keys check.
        /// </summary>
        /// <param name="cmd">The command.</param>
        public override void EnableForeignKeysCheck(IDbCommand cmd)
        {
            cmd.ExecNonQuery("EXEC sp_msforeachtable \"ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all\"");
        }

        /// <summary>
        /// Enables the foreign keys check asynchronous.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public override Task EnableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default)
        {
            return cmd.ExecNonQueryAsync(
                "EXEC sp_msforeachtable \"ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all\"",
                null,
                token);
        }

        /// <summary>
        /// Disables the foreign keys check.
        /// </summary>
        /// <param name="cmd">The command.</param>
        public override void DisableForeignKeysCheck(IDbCommand cmd)
        {
            cmd.ExecNonQuery("EXEC sp_msforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"");
        }

        /// <summary>
        /// Disables the foreign keys check asynchronous.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public override Task DisableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default)
        {
            return cmd.ExecNonQueryAsync("EXEC sp_msforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"", null,
                token);
        }

        /// <summary>
        /// Unwraps the specified database.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <returns>SqlConnection.</returns>
        static protected DbConnection Unwrap(IDbConnection db)
        {
            return (DbConnection)db.ToDbConnection();
        }

        /// <summary>
        /// Unwraps the specified command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns>SqlCommand.</returns>
        static protected DbCommand Unwrap(IDbCommand cmd)
        {
            return (DbCommand)cmd.ToDbCommand();
        }

        /// <summary>
        /// Unwraps the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>SqlDataReader.</returns>
        static protected DbDataReader Unwrap(IDataReader reader)
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
        public async override Task<List<T>> ReaderEach<T>(
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

        /// <summary>
        /// Readers the each.
        /// </summary>
        /// <typeparam name="Return">The type of the return.</typeparam>
        /// <param name="reader">The reader.</param>
        /// <param name="fn">The function.</param>
        /// <param name="source">The source.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Return&gt;.</returns>
        public async override Task<Return> ReaderEach<Return>(
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
        /// Initializes the connection.
        /// </summary>
        /// <param name="dbConn">The database connection.</param>
        public override void InitConnection(IDbConnection dbConn)
        {
            if (dbConn is OrmLiteConnection ormLiteConn && dbConn.ToDbConnection() is SqlConnection sqlConn)
            {
                ormLiteConn.ConnectionId = sqlConn.ClientConnectionId;
            }

            foreach (var command in this.ConnectionCommands)
            {
                using var cmd = dbConn.CreateCommand();
                cmd.ExecNonQuery(command);
            }

            this.OnOpenConnection?.Invoke(dbConn);
        }

        /// <summary>
        /// Gets the UTC date function.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string GetUtcDateFunction()
        {
            return "GETUTCDATE()";
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
            return $"DATEDIFF({interval}, {date1}, {date2})";
        }

        /// <summary>
        /// Gets the SQL ISNULL Function
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="alternateValue">The alternate Value.</param>
        /// <returns>The <see cref="string" />.</returns>
        public override string IsNullFunction(string expression, object alternateValue)
        {
            return $"ISNULL(({expression}), {alternateValue})";
        }

        /// <summary>
        /// Converts the flag.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string ConvertFlag(string expression)
        {
            return $"CONVERT([bit], sign({expression}))";
        }

        /// <summary>
        /// Databases the fragmentation information.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>System.String.</returns>
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

        /// <summary>
        /// Databases the size.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>System.String.</returns>
        public override string DatabaseSize(string database)
        {
            return "SELECT sum(reserved_page_count) * 8.0 / 1024 FROM sys.dm_db_partition_stats";
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
            return "Microsoft SQL Server";
        }

        /// <summary>
        /// Shrinks the database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>System.String.</returns>
        public override string ShrinkDatabase(string database)
        {
            return $"DBCC SHRINKDATABASE(N'{database}')";
        }

        /// <summary>
        /// Res the index database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="objectQualifier">The object qualifier.</param>
        /// <returns>System.String.</returns>
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

        /// <summary>
        /// Changes the recovery mode.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>System.String.</returns>
        public override string ChangeRecoveryMode(string database, string mode)
        {
            return $"ALTER DATABASE {database} SET RECOVERY {mode}";
        }

        /// <summary>
        /// Just runs the SQL command according to specifications.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Returns the Results</returns>
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
                                results.Append(',');
                                results.Append(n);
                            });

                        results.AppendLine();

                        while (reader.Read())
                        {
                            results.AppendFormat("""
                                                 "{0}"
                                                 """, rowIndex++);

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
