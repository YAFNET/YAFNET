// ***********************************************************************
// <copyright file="SqlServerOrmLiteDialectProvider.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using ServiceStack.OrmLite.Converters;
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
        public override DbKind Kind => DbKind.SqlServer;

        /// <summary>
        /// The instance
        /// </summary>
        public static SqlServerOrmLiteDialectProvider Instance = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerOrmLiteDialectProvider"/> class.
        /// </summary>
        public SqlServerOrmLiteDialectProvider()
        {
            this.VectorConverter = new VectorTextConverter();
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

            var sql = @"SELECT t.NAME, p.rows FROM sys.tables t INNER JOIN sys.schemas s ON t.schema_id = s.schema_id 
                               INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id 
                               INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
                         WHERE t.is_ms_shipped = 0 " + schemaSql + " GROUP BY t.NAME, p.Rows";
            return sql;
        }

        public override List<string> GetSchemas(IDbCommand dbCmd)
        {
            var sql = "SELECT DISTINCT TABLE_SCHEMA FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";
            return dbCmd.SqlColumn<string>(sql);
        }

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
            var sql = $"CREATE SCHEMA [{this.NamingStrategy.GetSchemaName(schemaName).Replace("]", "]]")}]";
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
            var tableName = this.GetTableNameOnly(tableRef);
            var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}".SqlFmt(this, tableName);

            var schema = this.GetSchemaName(tableRef);
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
            var tableName = this.GetTableNameOnly(tableRef);
            var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}".SqlFmt(this, tableName);

            var schema = this.GetSchemaName(tableRef);
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
            var tableName = this.GetTableNameOnly(tableRef);
            var sql =
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName"
                    .SqlFmt(this, tableName, columnName);

            var schema = this.GetSchemaName(tableRef);
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
            var tableName = this.GetTableNameOnly(tableRef);
            var sql =
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName"
                    .SqlFmt(this, tableName, columnName);

            var schema = this.GetSchemaName(tableRef);
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
            var tableName = this.GetTableNameOnly(tableRef);
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
            var tableName = this.GetTableNameOnly(tableRef);
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

            sb.Append("'");

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

        /// <summary>Gets the add composite primary key sql command.</summary>
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

        /// <summary>Gets the name of the primary key.</summary>
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
            sb.AppendFormat("and is_primary_key = 1");

            return StringBuilderCache.ReturnAndFree(sb);
        }

        /// <summary>
        /// Gets the drop primary key constraint.
        /// </summary>
        /// <param name="database">the database name</param>
        /// <param name="modelDef">The model definition.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        /// <summary>
        /// Gets the drop primary key constraint.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string GetDropPrimaryKeyConstraint(string database, ModelDefinition modelDef, string name)
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


        /// <summary>Gets the drop primary key constraint.</summary>
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
        public override string ToAddColumnStatement(TableRef tableRef, FieldDefinition fieldDef)
        {
            return $"ALTER TABLE {this.QuoteTable(tableRef)} ADD {this.GetColumnDefinition(fieldDef)};";
        }

        public override string ToAlterColumnStatement(TableRef tableRef, FieldDefinition fieldDef)
        {
            return $"ALTER TABLE {this.QuoteTable(tableRef)} ALTER COLUMN {this.GetColumnDefinition(fieldDef)};";
        }

        /// <summary>
        /// Converts to changecolumnnamestatement.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <param name="oldColumnName">Old name of the column.</param>
        /// <returns>System.String.</returns>
        public override string ToChangeColumnNameStatement(TableRef tableRef, FieldDefinition fieldDef, string oldColumn)
        {
            var objectName = $"{this.QuoteTable(tableRef)}.{this.GetQuotedColumnName(oldColumn)}";

            return
                $"EXEC sp_rename {this.GetQuotedValue(objectName)}, {this.GetQuotedValue(fieldDef.FieldName)}, {this.GetQuotedValue("COLUMN")};";
        }

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

        protected override bool SupportsIndexInclude => true;

        // System-versioned (temporal) tables need SQL Server 2016+
        public override bool SupportsSystemVersioning => true;

        // The period of a system-versioned table is in UTC
        public override DateTime ToSystemTime(DateTime time) =>
            time.Kind == DateTimeKind.Local ? time.ToUniversalTime() : time;

        protected override string GetSystemTimeColumnDefinition(FieldDefinition fieldDef) =>
            $"{this.GetQuotedColumnName(fieldDef)} DATETIME2 GENERATED ALWAYS AS ROW {(fieldDef.IsRowStart ? "START" : "END")} NOT NULL";

        // Previous versions are kept in a history table, in the table's schema
        private string GetHistoryTableName(ModelDefinition modelDef)
        {
            var tableRef = new TableRef(modelDef);
            return this.GetQuotedName(this.GetSchemaName(tableRef) ?? DefaultSchema) + "." +
                   this.GetQuotedName(modelDef.HistoryTable ?? this.GetTableNameOnly(tableRef) + "History");
        }

        protected override string ToSystemVersionedTableStatement(ModelDefinition modelDef, string createTableSql)
        {
            GetSystemTimeFields(modelDef, out var rowStart, out var rowEnd);
            var definitions = "";
            var startColumn = rowStart != null ? this.GetQuotedColumnName(rowStart) : this.GetQuotedName("SysStartTime");
            var endColumn = rowEnd != null ? this.GetQuotedColumnName(rowEnd) : this.GetQuotedName("SysEndTime");
            if (rowStart == null)
            {
                // The period needs columns, which aren't selected unless they're named
                definitions = $"{startColumn} DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,\n  " +
                              $"{endColumn} DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,\n  ";
            }
            definitions += $"PERIOD FOR SYSTEM_TIME ({startColumn}, {endColumn})";

            return AddToCreateTable(createTableSql, definitions,
                $" WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = {this.GetHistoryTableName(modelDef)}))");
        }

        // A system-versioned table can't be dropped until it stops being versioned, which leaves its history table
        public override string ToDropTableStatement(ModelDefinition modelDef)
        {
            var table = this.GetQuotedTableName(modelDef);
            if (!modelDef.IsSystemVersioned)
            {
                return $"DROP TABLE {table}";
            }

            return $"IF OBJECTPROPERTY(OBJECT_ID({this.GetQuotedValue(table)}), 'TableTemporalType') = 2 " +
                   $"ALTER TABLE {table} SET (SYSTEM_VERSIONING = OFF);\n" +
                   $"DROP TABLE {table};\n" +
                   $"DROP TABLE IF EXISTS {this.GetHistoryTableName(modelDef)};";
        }

        // Vectors use the VECTOR type of SQL Server 2025 and Azure SQL
        public override string GetVectorColumnDefinition(int dimensions) => $"VECTOR({dimensions})";

        public override string ToVectorParam(string param, int dimensions) => $"CAST({param} AS VECTOR({dimensions}))";

        // Half-precision vectors are a preview feature of SQL Server 2025, which needs PREVIEW_FEATURES to be enabled
        public override string GetVectorColumnDefinition(int dimensions, VectorPrecision precision) =>
            precision == VectorPrecision.Half ? $"VECTOR({dimensions}, float16)" : this.GetVectorColumnDefinition(dimensions);

        public override string ToVectorParam(string param, int dimensions, VectorPrecision precision) =>
            precision == VectorPrecision.Half ? $"CAST({param} AS VECTOR({dimensions}, float16))" : this.ToVectorParam(param, dimensions);

        public override string ToVectorDistance(VectorDistance distance, string vector, string other)
        {
            var metric = distance switch
            {
                VectorDistance.Cosine => "cosine",
                VectorDistance.L2 => "euclidean",
                _ => "dot",
            };
            return $"VECTOR_DISTANCE('{metric}', {vector}, {other})";
        }

        // Computed columns have the type of their expression
        protected override string GetGeneratedColumnDefinition(FieldDefinition fieldDef) =>
            $"{this.GetQuotedColumnName(fieldDef)} AS ({this.ResolveColumnRefs(fieldDef.ModelDef, fieldDef.ComputeExpression)})" +
            (fieldDef.IsPersisted ? " PERSISTED" : "");

        public override List<string> ToCreateCommentStatements(Type tableType)
        {
            var to = new List<string>();
            var modelDef = GetModel(tableType);
            var tableRef = new TableRef(modelDef);
            string N(string text) => "N" + this.GetQuotedValue(text);
            // Tables without a [Schema] are created in the connection's default schema, which isn't always dbo
            var schema = this.GetSchemaName(tableRef);
            var table = "@level0type=N'SCHEMA', @level0name=" + (schema != null ? N(schema) : "@schema") +
                        ", @level1type=N'TABLE', @level1name=" + N(this.GetTableNameOnly(tableRef));
            var addDescription = (schema == null ? "DECLARE @schema sysname = SCHEMA_NAME(); " : "") +
                                 "EXEC sp_addextendedproperty @name=N'MS_Description', @value=";

            if (!string.IsNullOrEmpty(modelDef.Description))
            {
                to.Add($"{addDescription}{N(modelDef.Description)}, {table}");
            }

            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (string.IsNullOrEmpty(fieldDef.Description) || fieldDef.ShouldSkipCreate())
                {
                    continue;
                }

                var column = this.GetQuotedColumnName(fieldDef).StripDbQuotes();
                to.Add($"{addDescription}{N(fieldDef.Description)}, {table}, @level2type=N'COLUMN', @level2name={N(column)}");
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
            // https://msdn.microsoft.com/en-us/library/ms182776.aspx
            if (fieldDef.IsRowVersion)
            {
                return $"{fieldDef.FieldName} rowversion NOT NULL";
            }

            if (fieldDef.IsRowStart || fieldDef.IsRowEnd)
            {
                return this.GetSystemTimeColumnDefinition(fieldDef);
            }

            if (fieldDef.IsGenerated)
            {
                return this.GetGeneratedColumnDefinition(fieldDef);
            }

            var fieldDefinition = this.GetFieldTypeDefinition(fieldDef);

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

            if (fieldDef.IsRowStart || fieldDef.IsRowEnd)
            {
                return this.GetSystemTimeColumnDefinition(fieldDef);
            }

            if (fieldDef.IsGenerated)
            {
                return this.GetGeneratedColumnDefinition(fieldDef);
            }

            var fieldDefinition = this.GetFieldTypeDefinition(fieldDef);

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

        /// <summary>
        /// Converts to dropconstraintstatement.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <param name="constraintName">Name of the constraint.</param>
        /// <returns>System.String.</returns>
        public override string ToDropConstraintStatement(TableRef tableRef, string constraintName) =>
            $"ALTER TABLE {this.QuoteTable(tableRef)} DROP CONSTRAINT {this.GetQuotedName(constraintName)};";

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

            using var bulkCopy = this.CreateBulkCopy(db, objs, config, out var table);
            bulkCopy.WriteToServer(table);
        }

        // A temporary table of the connection
        protected override string GetBulkStagingTableName(string name) => this.GetQuotedName("#" + name);

        protected override string ToCreateTempTableStatement(string tempTable, string columnDefinitions) =>
            $"CREATE TABLE {tempTable} (\n  {columnDefinitions}\n)";

        public override List<string> GetTableIndexNames(IDbConnection db, TableRef tableRef) => db.Column<string>(
            "SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID({0}) AND name IS NOT NULL"
                .SqlFmt(this, this.QuoteTable(tableRef)));

        public override List<IndexSchema> GetTableIndexes(IDbConnection db, TableRef tableRef)
        {
            string Columns(int included, string orderBy) =>
                "STUFF((SELECT ',' + c.name FROM sys.index_columns ic " +
                "JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id " +
                $"WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.is_included_column = {included} " +
                $"ORDER BY {orderBy} FOR XML PATH('')), 1, 1, '')";
            return ToIndexSchemas(db.SqlList<Dictionary<string, object>>(
                ("SELECT i.name AS name, i.is_unique AS is_unique, " +
                 "CAST(CASE WHEN i.is_primary_key = 1 OR i.is_unique_constraint = 1 THEN 1 ELSE 0 END AS bit) AS is_constraint, " +
                 $"i.is_primary_key AS is_primary_key, {Columns(0, "ic.key_ordinal")} AS columns, " +
                 $"{Columns(1, "ic.index_column_id")} AS include, i.filter_definition AS where_condition " +
                 "FROM sys.indexes i WHERE i.object_id = OBJECT_ID({0}) AND i.name IS NOT NULL")
                    .SqlFmt(this, this.QuoteTable(tableRef))));
        }

        public override List<string> GetModelIndexConditions(IDbConnection db, List<FieldDefinition> fieldDefs,
            List<IndexSchema> indexes) => this.ReadModelIndexConditions(db, fieldDefs, indexes, tempTable => ToColumnDefaults(
                db.SqlList<Dictionary<string, object>>(
                    "SELECT name AS name, filter_definition AS value FROM tempdb.sys.indexes WHERE object_id = OBJECT_ID({0})"
                        .SqlFmt(this, "tempdb.." + tempTable.StripDbQuotes()))));

        // The catalog full-text indexes are created in
        public const string FullTextCatalog = "ormlite_fts";

        // A full-text index keyed by the table's primary key. Its catalog and index can't be created in a transaction, so
        // they're created with EXEC, which runs them in batches of their own.
        public override List<string> ToCreateFullTextIndexStatements(ModelDefinition modelDef)
        {
            if (modelDef.PrimaryKey == null)
            {
                throw new NotSupportedException($"SQL Server's full-text index of {modelDef.Name} needs a primary key");
            }

            var table = this.GetQuotedTableName(modelDef);
            var language = modelDef.FullTextIndex?.Language;
            var languageTerm = string.IsNullOrEmpty(language) ? "0"
                : int.TryParse(language, out var lcid) ? lcid.ToString() : this.GetQuotedValue(language);
            var columns = this.GetFullTextColumnNames(modelDef).Map(x => $"{x} LANGUAGE {languageTerm}").Join(", ");
            var catalog = this.GetQuotedName(FullTextCatalog);
            return [
                $"IF NOT EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE name = {this.GetQuotedValue(FullTextCatalog)}) " +
                $"EXEC({this.GetQuotedValue($"CREATE FULLTEXT CATALOG {catalog}")});",
                $"DECLARE @key sysname = (SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID({this.GetQuotedValue(table)}) AND is_primary_key = 1);\n" +
                $"DECLARE @sql nvarchar(max) = {this.GetQuotedValue($"CREATE FULLTEXT INDEX ON {table} ({columns}) KEY INDEX ")} + QUOTENAME(@key) + " +
                $"{this.GetQuotedValue($" ON {catalog} WITH (CHANGE_TRACKING = AUTO, STOPLIST = OFF)")};\n" +
                "EXEC(@sql);",
            ];
        }

        public override List<string> ToDropFullTextIndexStatements(ModelDefinition modelDef)
        {
            var table = this.GetQuotedTableName(modelDef);
            return [
                $"IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID({this.GetQuotedValue(table)})) " +
                $"EXEC({this.GetQuotedValue($"DROP FULLTEXT INDEX ON {table}")});",
            ];
        }

        // Full-Text Search is an optional component of SQL Server
        public override bool SupportsFullTextSearch(IDbConnection db) =>
            db.Scalar<int?>("SELECT CONVERT(int, FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))") == 1;

        public override bool HasFullTextIndex(IDbConnection db, ModelDefinition modelDef) =>
            db.Scalar<int>("SELECT COUNT(*) FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID({0})"
                .SqlFmt(this, this.GetQuotedTableName(modelDef))) > 0;

        // Rows are indexed in the background after they're written
        public override bool IsFullTextIndexUpToDate(IDbConnection db, ModelDefinition modelDef)
        {
            var objectId = "OBJECT_ID({0})".SqlFmt(this, this.GetQuotedTableName(modelDef));
            return db.Scalar<int>($"SELECT CASE WHEN OBJECTPROPERTYEX({objectId}, 'TableFulltextPendingChanges') = 0 " +
                $"AND OBJECTPROPERTYEX({objectId}, 'TableFulltextPopulateStatus') = 0 THEN 1 ELSE 0 END") == 1;
        }

        // CONTAINS() of several columns only matches rows with every word of its search in the same column, so each word
        // and phrase is matched by a CONTAINS() of its own, which matches it in any column
        public override bool FullTextMatchesEachTerm => true;

        // Words are prefixes of the words they match, e.g. "data*"
        public override List<string> ToFullTextSearch(List<FullTextTerm> terms) =>
            terms.Map(x => x.IsPhrase ? "\"" + x.Words.Join(" ") + "\"" : "\"" + x.Words[0] + "*\"");

        public override string ToFullTextMatch(FullTextColumns columns, List<string> parameters) =>
            "(" + parameters.Map(p => $"CONTAINS(({columns.Columns.Join(", ")}), {p})").Join(" AND ") + ")";

        // The sum of the rank of each word and phrase
        public override string ToFullTextRank(FullTextColumns columns, List<string> parameters)
        {
            var ftColumns = this.GetFullTextColumnNames(columns.ModelDef).Join(", ");
            return "(" + parameters.Map(p => $"COALESCE((SELECT ft.[RANK] FROM CONTAINSTABLE({columns.Table}, ({ftColumns}), {p}) ft " +
                $"WHERE ft.[KEY] = {columns.PrimaryKey}), 0)").Join(" + ") + ")";
        }

        // Temporary tables, e.g. of GetModelCheckConstraints(), are in tempdb
        public override List<CheckConstraintSchema> GetCheckConstraints(IDbConnection db, string quotedTable)
        {
            var isTemp = quotedTable.IndexOf('#') >= 0;
            var catalog = isTemp ? "tempdb." : "";
            var objectName = isTemp ? "tempdb.." + quotedTable.StripDbQuotes() : quotedTable;
            return ToCheckConstraintSchemas(db.SqlList<Dictionary<string, object>>(
                ($"SELECT cc.name AS name, cc.definition AS condition FROM {catalog}sys.check_constraints cc " +
                 "WHERE cc.parent_object_id = OBJECT_ID({0})").SqlFmt(this, objectName)));
        }

        // Temporary tables, e.g. of GetModelSchemaColumns(), are in tempdb
        public override Dictionary<string, string> GetColumnDefaults(IDbConnection db, string quotedTable)
        {
            var isTemp = quotedTable.IndexOf('#') >= 0;
            var catalog = isTemp ? "tempdb." : "";
            var objectName = isTemp ? "tempdb.." + quotedTable.StripDbQuotes() : quotedTable;
            return ToColumnDefaults(db.SqlList<Dictionary<string, object>>(
                ($"SELECT c.name AS name, dc.definition AS value FROM {catalog}sys.columns c " +
                 $"LEFT JOIN {catalog}sys.default_constraints dc ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id " +
                 "WHERE c.object_id = OBJECT_ID({0})").SqlFmt(this, objectName)));
        }

        public override List<ForeignKeySchema> GetTableForeignKeys(IDbConnection db, TableRef tableRef)
        {
            string Columns(string table, string column) =>
                "STUFF((SELECT ',' + c.name FROM sys.foreign_key_columns fkc " +
                $"JOIN sys.columns c ON c.object_id = fkc.{table} AND c.column_id = fkc.{column} " +
                "WHERE fkc.constraint_object_id = fk.object_id ORDER BY fkc.constraint_column_id FOR XML PATH('')), 1, 1, '')";
            return ToForeignKeySchemas(db.SqlList<Dictionary<string, object>>(
                ($"SELECT fk.name AS name, {Columns("parent_object_id", "parent_column_id")} AS columns, " +
                 $"OBJECT_NAME(fk.referenced_object_id) AS ref_table, {Columns("referenced_object_id", "referenced_column_id")} AS ref_columns, " +
                 "fk.delete_referential_action_desc AS on_delete, fk.update_referential_action_desc AS on_update " +
                 "FROM sys.foreign_keys fk WHERE fk.parent_object_id = OBJECT_ID({0})").SqlFmt(this, this.QuoteTable(tableRef))));
        }

        // Defaults are constraints, so the column's default is dropped by its name before it's added
        public override string ToAlterColumnDefaultStatement(TableRef tableRef, FieldDefinition fieldDef)
        {
            var table = this.QuoteTable(tableRef);
            var sql = "DECLARE @default sysname = (SELECT dc.name FROM sys.default_constraints dc " +
                "JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id " +
                "WHERE dc.parent_object_id = OBJECT_ID({0}) AND c.name = {1});\n".SqlFmt(this, table, this.NamingStrategy.GetColumnName(fieldDef.FieldName)) +
                $"DECLARE @drop nvarchar(max) = {this.GetQuotedValue($"ALTER TABLE {table} DROP CONSTRAINT ")} + QUOTENAME(@default);\n" +
                "IF @default IS NOT NULL EXEC(@drop);";
            var defaultValue = this.GetDefaultValue(fieldDef);
            if (string.IsNullOrEmpty(defaultValue))
            {
                return sql;
            }

            var constraint = fieldDef.DefaultValueConstraint != null ? $" CONSTRAINT {this.GetQuotedName(fieldDef.DefaultValueConstraint)}" : "";
            return sql + $"\nALTER TABLE {table} ADD{constraint} DEFAULT {defaultValue} FOR {this.GetQuotedColumnName(fieldDef)};";
        }

        // UNION ALL stops the staging table inheriting the IDENTITY of the table's column, so it can be given values
        protected override string ToCreateBulkStagingTableStatement(ModelDefinition modelDef, string stagingTable, List<FieldDefinition> fieldDefs)
        {
            var columns = fieldDefs.Map(this.GetQuotedColumnName).Join(",");
            var table = this.GetQuotedTableName(modelDef);
            return $"SELECT {columns} INTO {stagingTable} FROM {table} WHERE 1=0 " +
                   $"UNION ALL SELECT {columns} FROM {table} WHERE 1=0";
        }

        protected override string ToBulkUpsertStatement(ModelDefinition modelDef, string stagingTable,
            List<FieldDefinition> insertFieldDefs, List<FieldDefinition> updateFieldDefs)
        {
            var quotedPrimaryKey = this.GetQuotedColumnName(modelDef.PrimaryKey);
            var whenMatched = updateFieldDefs.Count > 0
                ? "WHEN MATCHED THEN UPDATE SET " + updateFieldDefs.Map(x =>
                    $"target.{this.GetQuotedColumnName(x)}=" + GetBulkUpsertValue(x.CustomUpdate, "source." + this.GetQuotedColumnName(x))).Join(", ") + " "
                : "";

            return $"MERGE INTO {this.GetQuotedTableName(modelDef)} WITH (HOLDLOCK) AS target " +
                   $"USING {stagingTable} AS source " +
                   $"ON target.{quotedPrimaryKey}=source.{quotedPrimaryKey} " +
                   whenMatched +
                   $"WHEN NOT MATCHED THEN INSERT ({insertFieldDefs.Map(this.GetQuotedColumnName).Join(",")}) " +
                   $"VALUES ({insertFieldDefs.Map(x => GetBulkUpsertValue(x.CustomInsert, "source." + this.GetQuotedColumnName(x))).Join(",")});";
        }

        // Rows are loaded with SqlBulkCopy
        protected override void BulkLoad<T>(IDbConnection db, string quotedTable, List<FieldDefinition> fieldDefs, IEnumerable<T> rows, BulkInsertConfig config)
        {
            if (config.Mode == BulkInsertMode.Sql)
            {
                base.BulkLoad(db, quotedTable, fieldDefs, rows, config);
                return;
            }

            using var bulkCopy = this.CreateBulkCopy(db, quotedTable, fieldDefs, rows, config, this.GetBulkLoadValue, out var table);
            bulkCopy.WriteToServer(table);
        }

        private SqlBulkCopy CreateBulkCopy<T>(IDbConnection db, IEnumerable<T> objs, BulkInsertConfig config, out DataTable table)
        {
            var modelDef = ModelDefinition<T>.Definition;
            var fieldDefs = this.GetInsertFieldDefinitions(modelDef, insertFields: config.InsertFields)
                .Where(x => !this.ShouldSkipInsert(x) || x.AutoId)
                .ToList();

            return this.CreateBulkCopy(db, modelDef.ModelName, fieldDefs, objs, config,
                (fieldDef, obj) => fieldDef.AutoId ? this.GetInsertDefaultValue(fieldDef) : fieldDef.GetValue(obj), out table);
        }

        private SqlBulkCopy CreateBulkCopy<T>(IDbConnection db, string quotedTable, List<FieldDefinition> fieldDefs,
            IEnumerable<T> objs, BulkInsertConfig config, Func<FieldDefinition, object, object> getValue, out DataTable table)
        {
            var sqlConn = (SqlConnection)db.ToDbConnection();
            // Rows are copied in the connection's transaction, which SqlBulkCopy doesn't use unless it's given it
            var sqlTrans = db.GetTransaction()?.ToDbTransaction() as SqlTransaction;
            var bulkCopy = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.Default, sqlTrans);

            bulkCopy.BatchSize = config.BatchSize;
            bulkCopy.DestinationTableName = quotedTable;

            table = new DataTable();
            foreach (var fieldDef in fieldDefs)
            {
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
                    var value = getValue(fieldDef, obj);

                    var converter = this.GetConverterBestMatch(fieldDef);
                    var dbValue = converter.ToDbValue(fieldDef.FieldType, value);
                    var columnName = this.NamingStrategy.GetColumnName(fieldDef.FieldName);
                    dbValue ??= DBNull.Value;
                    row[columnName] = dbValue;
                }

                table.Rows.Add(row);
            }

            return bulkCopy;
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
                        sbReturningColumns.Append(",");
                    }

                    sbReturningColumns.Append("INSERTED." + this.GetQuotedColumnName(fieldDef));
                }

                if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
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

            foreach (var fieldDef in modelDef.AutoIdFields)
            {
                // need to include any AutoId fields that weren't included
                if (fieldDefs.Contains(fieldDef))
                {
                    continue;
                }

                if (sbReturningColumns.Length > 0)
                {
                    sbReturningColumns.Append(",");
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
            return QuoteSchemaName(NamingStrategy.GetSchemaName(schema)) + "." + this.GetQuotedName(sequence);
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
        /// Gets the table hint for SqlExpression.ForUpdate(). UPDLOCK holds the lock until the end of the transaction,
        /// READPAST skips locked rows
        /// </summary>
        /// <param name="skipLocked">if set to <c>true</c> [skip locked].</param>
        /// <returns>System.String.</returns>
        public override string GetForUpdateTableHint(bool skipLocked)
        {
            return skipLocked ? "WITH (UPDLOCK, ROWLOCK, READPAST)" : "WITH (UPDLOCK, ROWLOCK)";
        }

        /// <summary>
        /// Gets the lock clause for SqlExpression.ForUpdate(), SQL Server uses a table hint instead
        /// </summary>
        /// <param name="lockTable">The lock table.</param>
        /// <param name="skipLocked">if set to <c>true</c> [skip locked].</param>
        /// <returns>System.String.</returns>
        public override string GetForUpdateClause(string lockTable, bool skipLocked)
        {
            return null;
        }

        /// <summary>
        /// Determines whether [has insert return values] [the specified model definition].
        /// </summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns><c>true</c> if [has insert return values] [the specified model definition]; otherwise, <c>false</c>.</returns>
        public override bool HasInsertReturnValues(ModelDefinition modelDef)
        {
            return modelDef.FieldDefinitions.Any(x => x.ReturnOnInsert || x.AutoId && x.FieldType == typeof(Guid));
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
            cmd.Parameters.Clear(); // a SET that's sent with params only lasts for its statement
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
            cmd.Parameters.Clear(); // a SET that's sent with params only lasts for its statement
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
            cmd.Parameters.Clear(); // a SET that's sent with params only lasts for its statement
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
            cmd.Parameters.Clear(); // a SET that's sent with params only lasts for its statement
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
                        sbReturningColumns.Append(",");
                    }

                    sbReturningColumns.Append("INSERTED." + this.GetQuotedColumnName(fieldDef));
                }

                if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId && shouldInclude?.Invoke(fieldDef) != true)
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
                    sbReturningColumns.Append(",");
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

        public override bool SupportsUpsert => true;

        // MERGE ... OUTPUT INSERTED.* returns the row as it is after the insert or update
        public override string ToUpsertReturningStatement(string sql, ModelDefinition modelDef, ICollection<FieldDefinition> returnFields = null) =>
            sql.TrimEnd().TrimEnd(';') + " " + this.GetOutputClause(modelDef, "INSERTED", returnFields) + ";";

        /// <summary>
        /// OUTPUT clause of all the table's columns, or only the returnFields
        /// </summary>
        private string GetOutputClause(ModelDefinition modelDef, string prefix, ICollection<FieldDefinition> returnFields)
        {
            var returnAll = returnFields == null || returnFields.Count == 0;
            var sb = StringBuilderCache.Allocate();
            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (fieldDef.CustomSelect != null || (!returnAll && !returnFields.Contains(fieldDef)))
                {
                    continue;
                }

                sb.Append(sb.Length == 0 ? "OUTPUT " : ", ").Append(prefix).Append('.').Append(this.GetQuotedColumnName(fieldDef));
            }
            if (sb.Length == 0)
            {
                throw new ArgumentException($"No columns of {modelDef.Name} to return", nameof(returnFields));
            }

            return StringBuilderCache.ReturnAndFree(sb);
        }

        public override void PrepareParameterizedUpsertStatement<T>(IDbCommand cmd,
            ICollection<string> insertFields = null, ICollection<string> updateOnly = null)
        {
            this.PrepareUpsertFields<T>(cmd, insertFields, updateOnly,
                out var modelDef, out var insertFieldDefs, out var updateFieldDefs);

            var primaryKey = modelDef.PrimaryKey;
            var quotedPrimaryKey = this.GetQuotedColumnName(primaryKey);
            var primaryKeyParam = this.GetParam(this.SanitizeFieldNameForParamName(primaryKey.FieldName));
            var insertColumns = insertFieldDefs.Map(this.GetQuotedColumnName).Join(",");
            var insertValues = insertFieldDefs.Map(x =>
                this.GetParam(this.SanitizeFieldNameForParamName(x.FieldName), x.CustomInsert)).Join(",");
            var whenMatched = updateFieldDefs.Count > 0
                ? "WHEN MATCHED THEN UPDATE SET " + this.GetUpsertUpdateSql(updateFieldDefs, "target.") + " "
                : "";

            cmd.CommandText = $"MERGE INTO {this.GetQuotedTableName(modelDef)} WITH (HOLDLOCK) AS target " +
                              $"USING (VALUES ({primaryKeyParam})) AS source ({quotedPrimaryKey}) " +
                              $"ON target.{quotedPrimaryKey}=source.{quotedPrimaryKey} " +
                              whenMatched +
                              $"WHEN NOT MATCHED THEN INSERT ({insertColumns}) VALUES ({insertValues});";
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
                        sbReturningColumns.Append(",");
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
                    sbColumnNames.Append(",");
                }

                if (sbColumnValues.Length > 0)
                {
                    sbColumnValues.Append(",");
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
                $"SELECT * FROM (SELECT {selectExpression.Substring(selectType.Length)}, ROW_NUMBER() OVER ({orderByExpression}) As RowNum {bodyExpression}) AS RowConstrainedResult WHERE RowNum > {skip} AND RowNum <= {row}";

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

            return selectType + " TOP " + take + sql.Substring(selectType.Length);
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
                [this.GetQuotedValue(currencySymbol), $"CONVERT(VARCHAR, CONVERT(MONEY, {fieldOrValue}), 1)"]);
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
                rows != null ? "OFFSET " + offset.GetValueOrDefault() + " ROWS FETCH NEXT " + rows + " ROWS ONLY" :
                "OFFSET " + offset.GetValueOrDefault(int.MaxValue) + " ROWS";
        }

        /// <summary>
        /// The query that returns the query plan of the SQL.
        /// SHOWPLAN_TEXT returns the estimated plan without running the query, but not for statements with params,
        /// so they're merged into the SQL. STATISTICS PROFILE runs the query and returns the plan with actual row
        /// counts after its results.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="analyze">if set to <c>true</c> [analyze].</param>
        /// <returns>ExplainQuery.</returns>
        public override ExplainQuery ToExplainQuery(IDbConnection db, string sql, bool analyze) => analyze
            ? new()
            {
                Sql = sql,
                BeforeSql = "SET STATISTICS PROFILE ON",
                AfterSql = "SET STATISTICS PROFILE OFF",
                ReadPlan = reader => ExplainQuery.ReadTable(reader, "Rows", "Executes", "StmtText")
            }
            : new()
            {
                Sql = sql,
                BeforeSql = "SET SHOWPLAN_TEXT ON",
                AfterSql = "SET SHOWPLAN_TEXT OFF",
                MergeParams = true
            };

        /// <summary>
        /// Adds an OUTPUT clause before the statement's WHERE clause, e.g:
        /// UPDATE "Table" SET ... OUTPUT INSERTED."Id", ... WHERE ...
        /// Note: SQL Server doesn't allow OUTPUT without INTO on tables with enabled triggers
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="modelDef">The model definition.</param>
        /// <param name="isDelete">if set to <c>true</c> [is delete].</param>
        /// <param name="returnFields">The fields to return, or null for all fields.</param>
        /// <returns>System.String.</returns>
        public override string ToReturningStatement(string sql, ModelDefinition modelDef, bool isDelete, ICollection<FieldDefinition> returnFields = null)
        {
            var output = this.GetOutputClause(modelDef, isDelete ? "DELETED" : "INSERTED", returnFields);

            sql = sql.TrimEnd().TrimEnd(';');

            // DELETE with joins, e.g. DELETE "Table" FROM "Table" INNER JOIN ... needs OUTPUT before FROM
            var isDeleteWithJoin = isDelete && !sql.TrimStart().StartsWith("DELETE FROM", StringComparison.OrdinalIgnoreCase);
            var index = IndexOfTopLevelKeyword(sql, isDeleteWithJoin ? "FROM" : "WHERE");
            return index < 0
                       ? sql + " " + output
                       : sql.Substring(0, index) + output + " " + sql.Substring(index);
        }

        /// <summary>
        /// Index of the first keyword outside of quotes and parentheses, or -1
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>System.Int32.</returns>
        private static int IndexOfTopLevelKeyword(string sql, string keyword)
        {
            var depth = 0;
            char quote = default;
            for (var i = 0; i < sql.Length; i++)
            {
                var c = sql[i];
                if (quote != default)
                {
                    if (c == quote)
                    {
                        quote = default;
                    }

                    continue;
                }

                switch (c)
                {
                    case '\'': case '"': quote = c; continue;
                    case '[': quote = ']'; continue;
                    case '(': depth++; continue;
                    case ')': depth--; continue;
                }

                if (depth == 0
                    && string.Compare(sql, i, keyword, 0, keyword.Length, StringComparison.OrdinalIgnoreCase) == 0
                    && (i == 0 || !char.IsLetterOrDigit(sql[i - 1]))
                    && (i + keyword.Length >= sql.Length || !char.IsLetterOrDigit(sql[i + keyword.Length])))
                {
                    return i;
                }
            }

            return -1;
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
            foreach (var entry in this.DateFormatMap)
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
        /// Errors where SQL Server confirmed the statement wasn't applied: deadlock and snapshot update conflict
        /// victims, In-Memory OLTP conflicts and Azure SQL throttling, failovers and resource limits
        /// </summary>
        public static HashSet<int> NotAppliedErrors { get; } = [
            1205, 3960, 4060, 4221, 10928, 10929, 40501, 40613, 41301, 41302, 41305, 41325, 41839, 49918, 49919, 49920,
        ];

        /// <summary>
        /// Errors where the connection was lost, so the statement may have been applied
        /// </summary>
        public static HashSet<int> ConnectionLostErrors { get; } = [
            64, 121, 233, 10053, 10054, 10060, 40197,
        ];

        public override TransientError GetTransientError(Exception ex)
        {
            if (ex is SqlException sqlEx)
            {
                foreach (SqlError error in sqlEx.Errors)
                {
                    if (NotAppliedErrors.Contains(error.Number))
                    {
                        return TransientError.NotApplied;
                    }

                    if (ConnectionLostErrors.Contains(error.Number))
                    {
                        return TransientError.MaybeApplied;
                    }
                }
            }
            return base.GetTransientError(ex);
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
        protected DbConnection Unwrap(IDbConnection db)
        {
            return (DbConnection)db.ToDbConnection();
        }

        /// <summary>
        /// Unwraps the specified command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns>SqlCommand.</returns>
        protected DbCommand Unwrap(IDbCommand cmd)
        {
            return (DbCommand)cmd.ToDbCommand();
        }

        /// <summary>
        /// Unwraps the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>SqlDataReader.</returns>
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

                return default(T);
            }
            finally
            {
                reader.Dispose();
            }
        }

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
