// ***********************************************************************
// <copyright file="SqlServer2014OrmLiteDialectProvider.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite.SqlServer;

using System;

using ServiceStack.DataAnnotations;

/// <summary>
/// Class SqlServer2014OrmLiteDialectProvider.
/// Implements the <see cref="ServiceStack.OrmLite.SqlServer.SqlServer2012OrmLiteDialectProvider" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.SqlServer.SqlServer2012OrmLiteDialectProvider" />
public class SqlServer2014OrmLiteDialectProvider : SqlServer2012OrmLiteDialectProvider
{
    /// <summary>
    /// The instance
    /// </summary>
    public static new SqlServer2014OrmLiteDialectProvider Instance = new();

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

        var fieldDefinition = this.ResolveFragment(fieldDef.CustomFieldDefinition) ??
                              this.GetColumnTypeDefinition(fieldDef.ColumnType, fieldDef.FieldLength, fieldDef.Scale);

        var memTableAttrib = fieldDef.PropertyInfo?.ReflectedType.FirstAttribute<SqlServerMemoryOptimizedAttribute>();
        var isMemoryTable = memTableAttrib != null;

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

        var bucketCount = fieldDef.PropertyInfo?.FirstAttribute<SqlServerBucketCountAttribute>()?.Count;

        if (fieldDef.IsPrimaryKey)
        {
            if (isMemoryTable)
            {
                sql.Append(" NOT NULL PRIMARY KEY NONCLUSTERED");
            }
            else
            {
                sql.Append(" PRIMARY KEY");

                if (fieldDef.IsNonClustered)
                {
                    sql.Append(" NONCLUSTERED");
                }
            }

            if (fieldDef.AutoIncrement)
            {
                sql.Append(" ").Append(this.GetAutoIncrementDefinition(fieldDef));
            }

            if (isMemoryTable && bucketCount.HasValue)
            {
                sql.Append($" HASH WITH (BUCKET_COUNT = {bucketCount.Value})");
            }
        }
        else
        {
            if (isMemoryTable && bucketCount.HasValue)
            {
                sql.Append($" NOT NULL INDEX {this.GetQuotedColumnName("IDX_" + fieldDef.FieldName)}");

                if (fieldDef.IsNonClustered)
                {
                    sql.Append(" NONCLUSTERED");
                }

                sql.Append($" HASH WITH (BUCKET_COUNT = {bucketCount.Value})");
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
            if (fieldDef.DefaultValueConstraint != null)
            {
                sql.Append(" CONSTRAINT ").Append(GetQuotedName(fieldDef.DefaultValueConstraint));
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

        var fieldDefinition = this.ResolveFragment(fieldDef.CustomFieldDefinition) ??
                              this.GetColumnTypeDefinition(fieldDef.ColumnType, fieldDef.FieldLength, fieldDef.Scale);

        var memTableAttrib = fieldDef.PropertyInfo?.ReflectedType.FirstAttribute<SqlServerMemoryOptimizedAttribute>();
        var isMemoryTable = memTableAttrib != null;

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

        var bucketCount = fieldDef.PropertyInfo?.FirstAttribute<SqlServerBucketCountAttribute>()?.Count;

        if (modelDef.CompositePrimaryKeys.Count != 0)
        {
            sql.Append(fieldDef.IsNullable ? " NULL" : " NOT NULL");
        }
        else
        {
            if (fieldDef.IsPrimaryKey)
            {
                if (isMemoryTable)
                {
                    sql.Append(" NOT NULL PRIMARY KEY NONCLUSTERED");
                }
                else
                {
                    sql.Append(" PRIMARY KEY");

                    if (fieldDef.IsNonClustered)
                    {
                        sql.Append(" NONCLUSTERED");
                    }
                }

                if (fieldDef.AutoIncrement)
                {
                    sql.Append(" ").Append(this.GetAutoIncrementDefinition(fieldDef));
                }

                if (isMemoryTable && bucketCount.HasValue)
                {
                    sql.Append($" HASH WITH (BUCKET_COUNT = {bucketCount.Value})");
                }
            }
            else
            {
                if (isMemoryTable && bucketCount.HasValue)
                {
                    sql.Append($" NOT NULL INDEX {this.GetQuotedColumnName("IDX_" + fieldDef.FieldName)}");

                    if (fieldDef.IsNonClustered)
                    {
                        sql.Append(" NONCLUSTERED");
                    }

                    sql.Append($" HASH WITH (BUCKET_COUNT = {bucketCount.Value})");
                }
                else
                {
                    sql.Append(fieldDef.IsNullable ? " NULL" : " NOT NULL");
                }
            }
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
        var memoryTableAttrib = tableType.FirstAttribute<SqlServerMemoryOptimizedAttribute>();

        var modelDef = GetModel(tableType);

        if (fileTableAttrib == null)
        {
            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (fieldDef.CustomSelect != null || fieldDef.IsComputed && !fieldDef.IsPersisted)
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

                var refModelDef = OrmLiteUtils.GetModelDefinition(fieldDef.ForeignKey.ReferenceType);
                sbConstraints.Append(
                    $", \n\n  CONSTRAINT {this.GetQuotedName(fieldDef.ForeignKey.GetForeignKeyName(modelDef, refModelDef, this.NamingStrategy, fieldDef))} " +
                    $"FOREIGN KEY ({this.GetQuotedColumnName(fieldDef)}) " +
                    $"REFERENCES {this.GetQuotedTableName(refModelDef)} ({this.GetQuotedColumnName(refModelDef.PrimaryKey)})");

                sbConstraints.Append(this.GetForeignKeyOnDeleteClause(fieldDef.ForeignKey));
                sbConstraints.Append(this.GetForeignKeyOnUpdateClause(fieldDef.ForeignKey));
            }

            if (memoryTableAttrib != null)
            {
                var attrib = tableType.FirstAttribute<SqlServerMemoryOptimizedAttribute>();
                sbTableOptions.Append(" WITH (MEMORY_OPTIMIZED = ON");
                switch (attrib.Durability)
                {
                    case SqlServerDurability.SchemaOnly:
                        sbTableOptions.Append(", DURABILITY = SCHEMA_ONLY");
                        break;
                    case SqlServerDurability.SchemaAndData:
                        sbTableOptions.Append(", DURABILITY = SCHEMA_AND_DATA");
                        break;
                }
                sbTableOptions.Append(')');
            }
        }
        else
        {
            var hasFileTableDir = !string.IsNullOrEmpty(fileTableAttrib.FileTableDirectory);
            var hasFileTableCollateFileName = !string.IsNullOrEmpty(fileTableAttrib.FileTableCollateFileName);

            if (hasFileTableDir || hasFileTableCollateFileName)
            {
                sbTableOptions.Append(" WITH (");

                if (hasFileTableDir)
                {
                    sbTableOptions.Append($" FILETABLE_DIRECTORY = N'{fileTableAttrib.FileTableDirectory}'\n");
                }

                if (hasFileTableCollateFileName)
                {
                    if (hasFileTableDir)
                    {
                        sbTableOptions.Append(" ,");
                    }

                    sbTableOptions.Append($" FILETABLE_COLLATE_FILENAME = {fileTableAttrib.FileTableCollateFileName ?? "database_default" }\n");
                }
                sbTableOptions.Append(")");
            }
        }

        var uniqueConstraints = this.GetUniqueConstraints(modelDef);
        if (uniqueConstraints != null)
        {
            sbConstraints.Append(",\n" + uniqueConstraints);
        }

        var sql = $"CREATE TABLE {this.GetQuotedTableName(modelDef)} ";
        sql += fileTableAttrib != null
                   ? $"\n AS FILETABLE{StringBuilderCache.ReturnAndFree(sbTableOptions)};"
                   : $"\n(\n  {StringBuilderCache.ReturnAndFree(sbColumns)}{StringBuilderCacheAlt.ReturnAndFree(sbConstraints)} \n){StringBuilderCache.ReturnAndFree(sbTableOptions)}; \n";

        return sql;
    }
}