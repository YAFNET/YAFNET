// ***********************************************************************
// <copyright file="SqlServer2012OrmLiteDialectProvider.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.SqlServer;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.DataAnnotations;
using ServiceStack.Text;

/// <summary>
/// Class SqlServer2012OrmLiteDialectProvider.
/// Implements the <see cref="ServiceStack.OrmLite.SqlServer.SqlServerOrmLiteDialectProvider" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.SqlServer.SqlServerOrmLiteDialectProvider" />
public class SqlServer2012OrmLiteDialectProvider : SqlServerOrmLiteDialectProvider
{
    /// <summary>
    /// The instance
    /// </summary>
    public static new SqlServer2012OrmLiteDialectProvider Instance = new SqlServer2012OrmLiteDialectProvider();

    /// <summary>
    /// Doeses the sequence exist.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sequence">Name of the sequence.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public override bool DoesSequenceExist(IDbCommand dbCmd, string sequence)
    {
        var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM sys.sequences WHERE object_id=object_id({0})) THEN 1 ELSE 0 END"
            .SqlFmt(this, sequence);

        var result = dbCmd.ExecLongScalar(sql);

        return result == 1;
    }

    /// <summary>
    /// Does sequence exist as an asynchronous operation.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sequenceName">Name of the sequence.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
    public override async Task<bool> DoesSequenceExistAsync(IDbCommand dbCmd, string sequenceName, CancellationToken token = default)
    {
        var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM sys.sequences WHERE object_id=object_id({0})) THEN 1 ELSE 0 END"
            .SqlFmt(this, sequenceName);

        var result = await dbCmd.ExecLongScalarAsync(sql, token);

        return result == 1;
    }

    /// <summary>
    /// Gets the automatic increment definition.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    protected override string GetAutoIncrementDefinition(FieldDefinition fieldDef)
    {
        return !string.IsNullOrEmpty(fieldDef.Sequence)
                   ? $"DEFAULT NEXT VALUE FOR {Sequence(NamingStrategy.GetSchemaName(GetModel(fieldDef.PropertyInfo?.ReflectedType)), fieldDef.Sequence)}"
                   : AutoIncrementDefinition;
    }

    /// <summary>
    /// Shoulds the skip insert.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected override bool ShouldSkipInsert(FieldDefinition fieldDef) =>
        fieldDef.ShouldSkipInsert() && string.IsNullOrEmpty(fieldDef.Sequence);

    /// <summary>
    /// Supportses the sequences.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected override bool SupportsSequences(FieldDefinition fieldDef) =>
        !string.IsNullOrEmpty(fieldDef.Sequence);

    /// <summary>
    /// Converts to createsequencestatements.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public override List<string> ToCreateSequenceStatements(Type tableType)
    {
        var modelDef = GetModel(tableType);
        return SequenceList(tableType).Select(seq => $"CREATE SEQUENCE {Sequence(NamingStrategy.GetSchemaName(modelDef), seq)} AS BIGINT START WITH 1 INCREMENT BY 1 NO CACHE;").ToList();
    }

    /// <summary>
    /// Converts to createsequencestatement.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <param name="sequenceName">Name of the sequence.</param>
    /// <returns>System.String.</returns>
    public override string ToCreateSequenceStatement(Type tableType, string sequenceName)
    {
        var modelDef = GetModel(tableType);
        return $"CREATE SEQUENCE {Sequence(NamingStrategy.GetSchemaName(modelDef), sequenceName)} AS BIGINT START WITH 1 INCREMENT BY 1 NO CACHE;";
    }

    /// <summary>
    /// Sequences the list.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public override List<string> SequenceList(Type tableType)
    {
        var gens = new List<string>();
        var modelDef = GetModel(tableType);

        foreach (var fieldDef in modelDef.FieldDefinitions)
        {
            if (!string.IsNullOrEmpty(fieldDef.Sequence))
            {
                gens.AddIfNotExists(fieldDef.Sequence);
            }
        }
        return gens;
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
    public override string ToSelectStatement(QueryType queryType, ModelDefinition modelDef,
                                             string selectExpression,
                                             string bodyExpression,
                                             string orderByExpression = null,
                                             int? offset = null,
                                             int? rows = null,
                                             ISet<string> tags = null)
    {
        var sb = StringBuilderCache.Allocate();
        ApplyTags(sb, tags);

        sb.Append(selectExpression)
            .Append(bodyExpression);

        if (!string.IsNullOrEmpty(orderByExpression))
            sb.Append(orderByExpression);

        var skip = offset ?? 0;

        if (skip <= 0 && rows is not > 0)
        {
            return StringBuilderCache.ReturnAndFree(sb);
        }

        // Use TOP if offset is unspecified
        if (skip == 0)
        {
            var sql = StringBuilderCache.ReturnAndFree(sb);
            return SqlTop(sql, rows.GetValueOrDefault());
        }

        if (queryType != QueryType.Select && rows != 1)
        {
            return StringBuilderCache.ReturnAndFree(sb);
        }

        // ORDER BY mandatory when using OFFSET/FETCH NEXT
        if (orderByExpression.IsEmpty())
        {
            var orderBy = rows == 1 //Avoid for Single requests
                              ? "1"
                              : this.GetQuotedColumnName(modelDef, modelDef.PrimaryKey);

            sb.Append(" ORDER BY " + orderBy);
        }
        sb.Append(" ").Append(SqlLimit(offset, rows));

        return StringBuilderCache.ReturnAndFree(sb);
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
            return $"{fieldDef.FieldName} rowversion NOT NULL";

        var fieldDefinition = ResolveFragment(fieldDef.CustomFieldDefinition) ??
                              GetColumnTypeDefinition(fieldDef.ColumnType, fieldDef.FieldLength, fieldDef.Scale);

        var sql = StringBuilderCache.Allocate();
        sql.Append($"{GetQuotedColumnName(fieldDef.FieldName)} {fieldDefinition}");

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
                sql.Append(" ").Append(AutoIncrementDefinition);
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

        var defaultValue = GetDefaultValue(fieldDef);
        if (!string.IsNullOrEmpty(defaultValue))
        {
            sql.AppendFormat(DefaultValueFormat, defaultValue);
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
            return $"{fieldDef.FieldName} rowversion NOT NULL";

        var fieldDefinition = ResolveFragment(fieldDef.CustomFieldDefinition) ??
                              GetColumnTypeDefinition(fieldDef.ColumnType, fieldDef.FieldLength, fieldDef.Scale);

        var sql = StringBuilderCache.Allocate();
        sql.Append($"{GetQuotedColumnName(fieldDef.FieldName)} {fieldDefinition}");

        if (fieldDef.FieldType == typeof(string))
        {
            // https://msdn.microsoft.com/en-us/library/ms184391.aspx
            var collation = fieldDef.PropertyInfo?.FirstAttribute<SqlServerCollateAttribute>()?.Collation;
            if (!string.IsNullOrEmpty(collation))
            {
                sql.Append($" COLLATE {collation}");
            }
        }

        if (modelDef.CompositePrimaryKeys.Any())
        {
            sql.Append(fieldDef.IsNullable ? " NULL" : " NOT NULL");
        }
        else
        {
            if (fieldDef.IsPrimaryKey)
            {
                sql.Append(" PRIMARY KEY");

                if (fieldDef.IsNonClustered)
                    sql.Append(" NONCLUSTERED");

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

        var defaultValue = GetDefaultValue(fieldDef);
        if (!string.IsNullOrEmpty(defaultValue))
        {
            sql.AppendFormat(DefaultValueFormat, defaultValue);
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
        var sbTableOptions = StringBuilderCacheAlt.Allocate();

        var fileTableAttrib = tableType.FirstAttribute<SqlServerFileTableAttribute>();

        var modelDef = GetModel(tableType);

        if (fileTableAttrib == null)
        {
            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (fieldDef.CustomSelect != null || fieldDef.IsComputed && !fieldDef.IsPersisted)
                    continue;

                var columnDefinition = GetColumnDefinition(fieldDef, modelDef);

                if (columnDefinition == null)
                    continue;

                if (sbColumns.Length != 0)
                    sbColumns.Append(", \n  ");

                sbColumns.Append(columnDefinition);

                var sqlConstraint = GetCheckConstraint(modelDef, fieldDef);
                if (sqlConstraint != null)
                {
                    sbConstraints.Append(",\n" + sqlConstraint);
                }

                if (fieldDef.ForeignKey == null || OrmLiteConfig.SkipForeignKeys)
                    continue;

                var refModelDef = GetModel(fieldDef.ForeignKey.ReferenceType);
                sbConstraints.Append(
                    $", \n\n  CONSTRAINT {GetQuotedName(fieldDef.ForeignKey.GetForeignKeyName(modelDef, refModelDef, NamingStrategy, fieldDef))} " +
                    $"FOREIGN KEY ({GetQuotedColumnName(fieldDef.FieldName)}) " +
                    $"REFERENCES {GetQuotedTableName(refModelDef)} ({GetQuotedColumnName(refModelDef.PrimaryKey.FieldName)})");

                sbConstraints.Append(GetForeignKeyOnDeleteClause(fieldDef.ForeignKey));
                sbConstraints.Append(GetForeignKeyOnUpdateClause(fieldDef.ForeignKey));
            }
        }
        else
        {
            if (fileTableAttrib.FileTableDirectory != null || fileTableAttrib.FileTableCollateFileName != null)
            {
                sbTableOptions.Append(" WITH (");

                if (fileTableAttrib.FileTableDirectory != null)
                {
                    sbTableOptions.Append($" FILETABLE_DIRECTORY = N'{fileTableAttrib.FileTableDirectory}'\n");
                }

                if (fileTableAttrib.FileTableCollateFileName != null)
                {
                    if (fileTableAttrib.FileTableDirectory != null)
                        sbTableOptions.Append(" ,");

                    sbTableOptions.Append($" FILETABLE_COLLATE_FILENAME = {fileTableAttrib.FileTableCollateFileName ?? "database_default" }\n");
                }
                sbTableOptions.Append(")");
            }
        }

        var uniqueConstraints = GetUniqueConstraints(modelDef);
        if (uniqueConstraints != null)
        {
            sbConstraints.Append(",\n" + uniqueConstraints);
        }

        if (modelDef.CompositePrimaryKeys.Any())
        {
            sbConstraints.Append(",\n");

            var primaryKeyName = $"PK_{this.NamingStrategy.GetTableName(modelDef)}";

            sbConstraints.AppendFormat(" CONSTRAINT {0} PRIMARY KEY CLUSTERED  (", primaryKeyName);

            sbConstraints.Append(
                modelDef.CompositePrimaryKeys.FirstOrDefault().FieldNames.Map(f => modelDef.GetQuotedName(f, this))
                    .Join(","));

            sbConstraints.Append(") ");
        }

        var sql = $"CREATE TABLE {GetQuotedTableName(modelDef)} ";
        sql += fileTableAttrib != null
                   ? $"\n AS FILETABLE{StringBuilderCache.ReturnAndFree(sbTableOptions)};"
                   : $"\n(\n  {StringBuilderCache.ReturnAndFree(sbColumns)}{StringBuilderCacheAlt.ReturnAndFree(sbConstraints)} \n){StringBuilderCache.ReturnAndFree(sbTableOptions)}; \n";

        return sql;
    }

    /// <summary>
    /// Appends the field condition.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="cmd">The command.</param>
    public override void AppendFieldCondition(StringBuilder sqlFilter, FieldDefinition fieldDef, IDbCommand cmd)
    {
        if (isSpatialField(fieldDef))
        {
            // Append condition statement to determine if SqlGeometry or SqlGeography type is Equal
            // using the type's STEquals method
            //
            // SqlGeometry: https://msdn.microsoft.com/en-us/library/microsoft.sqlserver.types.sqlgeometry.stequals.aspx
            // SqlGeography: https://msdn.microsoft.com/en-us/library/microsoft.sqlserver.types.sqlgeography.stequals.aspx
            sqlFilter
                .Append(GetQuotedColumnName(fieldDef.FieldName))
                .Append(".STEquals(")
                .Append(this.GetParam(SanitizeFieldNameForParamName(fieldDef.FieldName)))
                .Append(") = 1");

            AddParameter(cmd, fieldDef);
        }
        else
        {
            base.AppendFieldCondition(sqlFilter, fieldDef, cmd);
        }
    }

    /// <summary>
    /// Appends the null field condition.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="fieldDef">The field definition.</param>
    public override void AppendNullFieldCondition(StringBuilder sqlFilter, FieldDefinition fieldDef)
    {
        if (hasIsNullProperty(fieldDef))
        {
            // Append condition statement to determine if SqlHierarchyId, SqlGeometry, or SqlGeography type is NULL
            // using the type's IsNull property
            //
            // SqlHierarchyId: https://msdn.microsoft.com/en-us/library/microsoft.sqlserver.types.sqlhierarchyid.isnull.aspx
            // SqlGeometry: https://msdn.microsoft.com/en-us/library/microsoft.sqlserver.types.sqlgeometry.isnull.aspx
            // SqlGeography: https://msdn.microsoft.com/en-us/library/microsoft.sqlserver.types.sqlgeography.isnull.aspx
            sqlFilter
                .Append(GetQuotedColumnName(fieldDef.FieldName))
                .Append(".IsNull = 1");
        }
        else
        {
            base.AppendNullFieldCondition(sqlFilter, fieldDef);
        }
    }

    /// <summary>
    /// Determines whether [is spatial field] [the specified field definition].
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns><c>true</c> if [is spatial field] [the specified field definition]; otherwise, <c>false</c>.</returns>
    internal bool isSpatialField(FieldDefinition fieldDef) =>
        fieldDef.FieldType.Name == "SqlGeography" || fieldDef.FieldType.Name == "SqlGeometry";

    /// <summary>
    /// Determines whether [has is null property] [the specified field definition].
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns><c>true</c> if [has is null property] [the specified field definition]; otherwise, <c>false</c>.</returns>
    internal bool hasIsNullProperty(FieldDefinition fieldDef) =>
        isSpatialField(fieldDef) || fieldDef.FieldType.Name == "SqlHierarchyId";
}