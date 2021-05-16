/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Data.MsSql
{
    using System;

    using ServiceStack;
    using ServiceStack.DataAnnotations;
    using ServiceStack.OrmLite;
    using ServiceStack.OrmLite.SqlServer;
    using ServiceStack.Text;

    using Config = YAF.Configuration.Config;

    /// <summary>
    /// The YAF SQL Server 2012 ORM lite dialect provider.
    /// </summary>
    public class YafSqlServer2012OrmLiteDialectProvider : SqlServer2012OrmLiteDialectProvider
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafSqlServer2012OrmLiteDialectProvider"/> class.
        /// </summary>
        public YafSqlServer2012OrmLiteDialectProvider()
        {
            this.NamingStrategy = new YafNamingStrategyBaseOverride();
        }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        public static new YafSqlServer2012OrmLiteDialectProvider Instance { get; set; } = new ();

        /// <summary>
        /// The to post create table statement.
        /// </summary>
        /// <param name="modelDef">
        /// The model definition.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToPostCreateTableStatement(ModelDefinition modelDef)
        {
            return modelDef.PostCreateTableSql?.Replace("{tableName}", this.GetTableName(modelDef.Name)).Replace(
                "{objectQualifier}",
                Config.DatabaseObjectQualifier).Replace("{databaseOwner}", Config.DatabaseOwner);
        }

        /// <summary>
        /// The to create table statement.
        /// </summary>
        /// <param name="tableType">
        /// The table type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToCreateTableStatement(Type tableType)
        {
            var columns = StringBuilderCache.Allocate();
            var constraints = StringBuilderCacheAlt.Allocate();
            var tableOptions = StringBuilderCacheAlt.Allocate();

            var fileTableAttributes = tableType.FirstAttribute<SqlServerFileTableAttribute>();

            var modelDef = GetModel(tableType);

            if (fileTableAttributes == null)
            {
                foreach (var fieldDef in modelDef.FieldDefinitions)
                {
                    if (fieldDef.CustomSelect != null || fieldDef.IsComputed && !fieldDef.IsPersisted)
                    {
                        continue;
                    }

                    var columnDefinition = this.GetColumnDefinition(fieldDef);

                    if (fieldDef.IsPrimaryKey)
                    {
                        columnDefinition = columnDefinition.Replace(
                            "PRIMARY KEY",
                            $"CONSTRAINT [PK_{this.NamingStrategy.GetTableName(modelDef.Name)}] PRIMARY KEY");
                    }

                    if (columnDefinition == null)
                    {
                        continue;
                    }

                    if (columns.Length != 0)
                    {
                        columns.Append(", \n  ");
                    }

                    columns.Append(columnDefinition);

                    var sqlConstraint = this.GetCheckConstraint(modelDef, fieldDef);

                    if (sqlConstraint != null)
                    {
                        constraints.Append(",\n" + sqlConstraint);
                    }

                    if (fieldDef.ForeignKey == null || OrmLiteConfig.SkipForeignKeys)
                    {
                        continue;
                    }

                    var refModelDef = GetModel(fieldDef.ForeignKey.ReferenceType);
                    constraints.Append(
                        $", \n\n  CONSTRAINT {this.GetQuotedName(fieldDef.ForeignKey.GetForeignKeyName(modelDef, refModelDef, this.NamingStrategy, fieldDef))} " +
                        $"FOREIGN KEY ({this.GetQuotedColumnName(fieldDef.FieldName)}) " +
                        $"REFERENCES {this.GetQuotedTableName(refModelDef)} ({this.GetQuotedColumnName(refModelDef.PrimaryKey.FieldName)})");

                    constraints.Append(this.GetForeignKeyOnDeleteClause(fieldDef.ForeignKey));
                    constraints.Append(this.GetForeignKeyOnUpdateClause(fieldDef.ForeignKey));
                }
            }
            else
            {
                if (fileTableAttributes.FileTableDirectory != null || fileTableAttributes.FileTableCollateFileName != null)
                {
                    tableOptions.Append(" WITH (");

                    if (fileTableAttributes.FileTableDirectory != null)
                    {
                        tableOptions.Append($" FILETABLE_DIRECTORY = N'{fileTableAttributes.FileTableDirectory}'\n");
                    }

                    if (fileTableAttributes.FileTableCollateFileName != null)
                    {
                        if (fileTableAttributes.FileTableDirectory != null)
                        {
                            tableOptions.Append(" ,");
                        }

                        tableOptions.Append($" FILETABLE_COLLATE_FILENAME = {fileTableAttributes.FileTableCollateFileName ?? "database_default" }\n");
                    }

                    tableOptions.Append(")");
                }
            }

            var uniqueConstraints = this.GetUniqueConstraints(modelDef);
            if (uniqueConstraints != null)
            {
                constraints.Append(",\n" + uniqueConstraints);
            }

            var sql = $"CREATE TABLE {this.GetQuotedTableName(modelDef)} ";
            sql += fileTableAttributes != null
                ? $"\n AS FILETABLE{StringBuilderCache.ReturnAndFree(tableOptions)};"
                : $"\n(\n  {StringBuilderCache.ReturnAndFree(columns)}{StringBuilderCacheAlt.ReturnAndFree(constraints)} \n){StringBuilderCache.ReturnAndFree(tableOptions)}; \n";

            return sql;
        }

        #endregion
    }
}