// ***********************************************************************
// <copyright file="OrmLiteSchemaModifyApi.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.DataAnnotations;
using System.Linq;

using ServiceStack.OrmLite.Base.Common;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

using System;
using System.Data;
using System.Linq.Expressions;
using System.Text;

/// <summary>
/// Enum OnFkOption
/// </summary>
public enum OnFkOption
{
    /// <summary>
    /// The cascade
    /// </summary>
    Cascade,

    /// <summary>
    /// The set null
    /// </summary>
    SetNull,

    /// <summary>
    /// The no action
    /// </summary>
    NoAction,

    /// <summary>
    /// The set default
    /// </summary>
    SetDefault,

    /// <summary>
    /// The restrict
    /// </summary>
    Restrict
}

/// <summary>
/// Class OrmLiteSchemaModifyApi.
/// </summary>
public static class OrmLiteSchemaModifyApi
{
    /// <summary>
    /// Initializes the user field definition.
    /// </summary>
    /// <param name="modelType">Type of the model.</param>
    /// <param name="fieldDef">The field definition.</param>
    private static void InitUserFieldDefinition(Type modelType, FieldDefinition fieldDef)
    {
        if (fieldDef.PropertyInfo == null)
        {
            fieldDef.PropertyInfo = TypeProperties.Get(modelType).GetPublicProperty(fieldDef.Name);
        }
    }

    /// <param name="dbConn">The database connection.</param>
    extension(IDbConnection dbConn)
    {
        /// <summary>
        /// Alters the table.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="command">The command.</param>
        public void AlterTable<T>(string command)
        {
            AlterTable(dbConn, typeof(T), command);
        }

        /// <summary>
        /// Alters the table.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="command">The command.</param>
        public void AlterTable(Type modelType, string command)
        {
            var sql = $"ALTER TABLE {dbConn.GetDialectProvider().GetQuotedTableName(modelType.GetModelDefinition())} {command};";
            dbConn.ExecuteSql(sql);
        }

        /// <summary>
        /// Adds the column with command.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="command">The command.</param>
        public void AddColumnWithCommand<T>(string command)
        {
            var modelDef = ModelDefinition<T>.Definition;

            var sql = $"ALTER TABLE {dbConn.GetDialectProvider().GetQuotedTableName(modelDef)} ADD {command};";
            dbConn.ExecuteSql(sql);
        }

        /// <summary>
        /// Return Quoted Table Name if not already quoted, uses naming strategy by default
        /// </summary>
        public TableRef QuoteTable(string tableName, bool useStrategy = true)
        {
            var dialect = dbConn.GetDialectProvider();
            return !OrmLiteUtils.IsQuoted(tableName)
                ? new TableRef(null, null)
                {
                    QuotedName = useStrategy
                        ? dialect.QuoteTable(tableName)
                        : dialect.GetQuotedName(tableName)
                }
                : new TableRef(null, null) { QuotedName = tableName };
        }

        /// <summary>
        /// Return Quoted Table Name if not already quoted, does not use naming strategy 
        /// </summary>
        public TableRef QuoteTableAlias(string tableName)
        {
            return QuoteTable(dbConn, tableName, useStrategy: false);
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="field">The field.</param>
        public void AddColumn<T>(Expression<Func<T, object>> field)
        {
            var modelDef = ModelDefinition<T>.Definition;
            var fieldDef = modelDef.GetFieldDefinition(field);

            if (fieldDef.Name != OrmLiteConfig.IdField)
            {
                fieldDef.IsPrimaryKey = false;
            }

            dbConn.AddColumn(typeof(T), fieldDef);
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef">The field definition.</param>
        public void AddColumn(Type modelType, FieldDefinition fieldDef)
        {
            InitUserFieldDefinition(modelType, fieldDef);

            var command = dbConn.GetDialectProvider().ToAddColumnStatement(modelType, fieldDef);
            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Alters the column.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="oldTablePrefix">The old table prefix.</param>
        public void AlterColumn<T>(Expression<Func<T, object>> field, string oldTablePrefix)
        {
            var columns = dbConn.GetDialectProvider().GetInsertColumnsStatement<T>();

            var modelDef = typeof(T).GetModelDefinition();

            var tableName = dbConn.GetDialectProvider().GetQuotedTableName(modelDef);
            var oldTableName = dbConn.GetDialectProvider().GetQuotedName($"{modelDef.Name}{oldTablePrefix}");

            dbConn.ExecuteSql(
                $@"BEGIN TRANSACTION;
                           ALTER TABLE {tableName} RENAME TO {oldTableName}; 
                       COMMIT;");

            dbConn.CreateTable<T>();

            dbConn.ExecuteSql(
                $@"BEGIN TRANSACTION;
                           INSERT INTO {tableName} ({columns})
                           SELECT {columns} FROM {oldTableName}; 
                       COMMIT;");

            dbConn.ExecuteSql(
                $@"DROP TABLE {oldTableName};");
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="fieldDef">The field definition.</param>
        public void AddColumn(TableRef tableRef, FieldDefinition fieldDef)
        {
            dbConn.ExecuteSql(dbConn.GetDialectProvider().ToAddColumnStatement(tableRef, fieldDef));
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <param name="fieldDef">The field definition.</param>
        public void AddColumn(string schema, string table, FieldDefinition fieldDef)
        {
            dbConn.ExecuteSql(dbConn.GetDialectProvider().ToAddColumnStatement(new TableRef(schema, table), fieldDef));
        }

        /// <summary>
        /// Alters the column.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="field">The field.</param>
        public void AlterColumn<T>(Expression<Func<T, object>> field)
        {
            var modelDef = ModelDefinition<T>.Definition;
            var fieldDef = modelDef.GetFieldDefinition(field);
            dbConn.AlterColumn(typeof(T), fieldDef);
        }

        /// <summary>
        /// Alters the column.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef">The field definition.</param>
        public void AlterColumn(Type modelType, FieldDefinition fieldDef)
        {
            InitUserFieldDefinition(modelType, fieldDef);

            dbConn.ExecuteSql(dbConn.GetDialectProvider().ToAlterColumnStatement(modelType, fieldDef));
        }

        /// <summary>
        /// Alters the column.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="fieldDef">The field definition.</param>
        public void AlterColumn(TableRef tableRef, FieldDefinition fieldDef)
        {
            dbConn.ExecuteSql(dbConn.GetDialectProvider().ToAlterColumnStatement(tableRef, fieldDef));
        }

        /// <summary>
        /// Alters the column.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <param name="fieldDef">The field definition.</param>
        public void AlterColumn(string schema, string table, FieldDefinition fieldDef)
        {
            dbConn.ExecuteSql(dbConn.GetDialectProvider().ToAlterColumnStatement(new TableRef(schema, table), fieldDef));
        }

        /// <summary>
        /// Changes the name of the column.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="oldColumn">Old name of the column.</param>
        public void ChangeColumnName<T>(Expression<Func<T, object>> field, string oldColumn)
        {
            var modelDef = ModelDefinition<T>.Definition;
            var fieldDef = modelDef.GetFieldDefinition<T>(field);
            dbConn.ChangeColumnName(typeof(T), fieldDef, oldColumn);
        }

        public void ChangeColumnName(TableRef tableRef, FieldDefinition fieldDef, string oldColumn)
        {
            dbConn.ExecuteSql(dbConn.GetDialectProvider().ToChangeColumnNameStatement(tableRef, fieldDef, oldColumn));
        }

        /// <summary>
        /// Changes the name of the column.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <param name="oldColumn">Old name of the column.</param>
        public void ChangeColumnName(Type modelType, FieldDefinition fieldDef, string oldColumn)
        {
            var command = dbConn.GetDialectProvider().ToChangeColumnNameStatement(modelType, fieldDef, oldColumn);
            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Renames the column.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field.</param>
        /// <param name="oldColumn">The old column.</param>
        public void RenameColumn<T>(Expression<Func<T, object>> field,
            string oldColumn)
        {
            var modelDef = ModelDefinition<T>.Definition;
            var fieldDef = modelDef.GetFieldDefinition(field);
            dbConn.RenameColumn(typeof(T), oldColumn, dbConn.GetNamingStrategy().GetColumnName(fieldDef.FieldName));
        }

        /// <summary>
        /// Renames the column.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldColumn">The old column.</param>
        /// <param name="newColumn">The new column.</param>
        public void RenameColumn<T>(string oldColumn, string newColumn)
        {
            dbConn.RenameColumn(typeof(T), oldColumn, newColumn);
        }

        /// <summary>
        /// Renames the column.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="oldColumn">The old column.</param>
        /// <param name="newColumn">The new column.</param>
        public void RenameColumn(Type modelType, string oldColumn, string newColumn)
        {
            dbConn.ExecuteSql(X.Map(dbConn.Dialect(), d => d.ToRenameColumnStatement(modelType,
                d.NamingStrategy.GetColumnName(oldColumn), d.NamingStrategy.GetColumnName(newColumn))));
        }

        /// <summary>
        /// Renames the column.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="oldColumn">The old column.</param>
        /// <param name="newColumn">The new column.</param>
        public void RenameColumn(TableRef tableRef, string oldColumn, string newColumn)
        {
            dbConn.ExecuteSql(dbConn.GetDialectProvider().ToRenameColumnStatement(tableRef, oldColumn, newColumn));
        }

        /// <summary>
        /// Renames the column.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <param name="oldColumn">The old column.</param>
        /// <param name="newColumn">The new column.</param>
        public void RenameColumn(string schema, string table, string oldColumn, string newColumn)
        {
            dbConn.ExecuteSql(X.Map(dbConn.Dialect(), d => d.ToRenameColumnStatement(new TableRef(schema, table),
                d.NamingStrategy.GetColumnName(oldColumn), d.NamingStrategy.GetColumnName(newColumn))));
        }

        /// <summary>
        /// Drops the column.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="field">The field.</param>
        public void DropColumn<T>(Expression<Func<T, object>> field)
        {
            var modelDef = ModelDefinition<T>.Definition;
            var fieldDef = modelDef.GetFieldDefinition(field);
            if (fieldDef.DefaultValueConstraint != null)
            {
                dbConn.DropConstraint(typeof(T), fieldDef.DefaultValueConstraint);
            }
            dbConn.DropColumn(typeof(T), fieldDef.FieldName);
        }

        /// <summary>
        /// Drops the column.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="column">Name of the column.</param>
        public void DropColumn<T>(string column)
        {
            dbConn.DropColumn(typeof(T), column);
        }

        public void DropColumn(TableRef tableRef, string column)
        {
            dbConn.ExecuteSql(dbConn.Dialect().ToDropColumnStatement(tableRef, column));
        }

        /// <summary>
        /// Drops the column.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="column">The column.</param>
        public void DropColumn(Type modelType, string column)
        {
            dbConn.ExecuteSql(X.Map(dbConn.Dialect(), d => d.ToDropColumnStatement(modelType, column)));
        }

        /// <summary>
        /// Drops the column.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <param name="column">The column.</param>
        public void DropColumn(string schema, string table, string column)
        {
            dbConn.ExecuteSql(X.Map(dbConn.Dialect(), d => d.ToDropColumnStatement(new TableRef(schema, table), column)));
        }

        public void DropConstraint(TableRef tableRef, string constraint)
        {
            dbConn.ExecuteSql(dbConn.Dialect().ToDropConstraintStatement(tableRef, constraint));
        }

        /// <summary>
        /// Drops the constraint.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="constraint">The constraint.</param>
        public void DropConstraint(Type modelType, string constraint)
        {
            // Many DBs don't support DROP CONSTRAINT
            var sql = X.Map(dbConn.Dialect(), d => d.ToDropConstraintStatement(modelType, constraint));
            if (!string.IsNullOrEmpty(sql))
            {
                dbConn.ExecuteSql(sql);
            }
        }

        /// <summary>
        /// Adds the foreign key.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <typeparam name="TForeign">The type of the t foreign.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="foreignField">The foreign field.</param>
        /// <param name="onUpdate">The on update.</param>
        /// <param name="onDelete">The on delete.</param>
        /// <param name="foreignKeyName">Name of the foreign key.</param>
        public void AddForeignKey<T, TForeign>(Expression<Func<T, object>> field,
            Expression<Func<TForeign, object>> foreignField,
            OnFkOption onUpdate,
            OnFkOption onDelete,
            string foreignKeyName = null)
        {
            var command = dbConn.GetDialectProvider().ToAddForeignKeyStatement(
                field, foreignField, onUpdate, onDelete, foreignKeyName);

            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Drops the primary key.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="fieldA">The field A.</param>
        /// <param name="fieldB">The field B.</param>
        public void AddCompositePrimaryKey<T>(Expression<Func<T, object>> fieldA, Expression<Func<T, object>> fieldB)
        {
            var provider = dbConn.GetDialectProvider();
            var modelDef = ModelDefinition<T>.Definition;

            var fieldDefA = modelDef.GetFieldDefinition(fieldA);
            var fieldNameA = provider.NamingStrategy.GetColumnName(fieldDefA.FieldName);

            var fieldDefB = modelDef.GetFieldDefinition(fieldB);
            var fieldNameB = provider.NamingStrategy.GetColumnName(fieldDefB.FieldName);

            var command = provider.GetAddCompositePrimaryKey(dbConn.Database, modelDef, fieldNameA, fieldNameB);

            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Gets the Primary Key name
        /// </summary>
        /// <typeparam name="T">The Model</typeparam>
        /// <returns>Returns the key name</returns>
        public string GetPrimaryKey<T>()
        {
            var provider = dbConn.GetDialectProvider();
            var modelDef = ModelDefinition<T>.Definition;

            var command = provider.GetPrimaryKeyName(modelDef);

            try
            {
                return dbConn.SqlScalar<string>(command);
            }
            catch (Exception)
            {
                return command;
            }
        }

        /// <summary>
        /// Drops the primary key.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="fieldA">The field A.</param>
        /// <param name="fieldB">The field B.</param>
        public void DropPrimaryKey<T>(string name, Expression<Func<T, object>> fieldA = null, Expression<Func<T, object>> fieldB = null)
        {
            var provider = dbConn.GetDialectProvider();
            var modelDef = ModelDefinition<T>.Definition;

            if (fieldA != null && fieldB != null)
            {
                var fieldDefA = modelDef.GetFieldDefinition(fieldA);
                var fieldNameA = provider.NamingStrategy.GetColumnName(fieldDefA.FieldName);

                var fieldDefB = modelDef.GetFieldDefinition(fieldB);
                var fieldNameB = provider.NamingStrategy.GetColumnName(fieldDefB.FieldName);

                var command = provider.GetDropPrimaryKeyConstraint(dbConn.Database, modelDef, name, fieldNameA, fieldNameB);

                dbConn.ExecuteSql(command);
            }
            else
            {
                var command = provider.GetDropPrimaryKeyConstraint(dbConn.Database, modelDef, name);

                dbConn.ExecuteSql(command);
            }
        }

        /// <summary>
        /// Drops the foreign key.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        public void DropForeignKeys<T>()
        {
            var provider = dbConn.GetDialectProvider();
            var modelDef = ModelDefinition<T>.Definition;

            var dropSql = provider.GetDropForeignKeyConstraints(modelDef);

            if (string.IsNullOrEmpty(dropSql))
            {
                throw new NotSupportedException($"Drop All Foreign Keys not supported by {provider.GetType().Name}");
            }

            dbConn.ExecuteSql(dropSql);
        }

        /// <summary>
        /// Drops the foreign key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="foreignKeyName">Name of the foreign key.</param>
        public void DropForeignKey<T>(string foreignKeyName)
        {
            var provider = dbConn.GetDialectProvider();
            var modelDef = ModelDefinition<T>.Definition;
            var dropSql = provider.ToDropForeignKeyStatement(new TableRef(modelDef), foreignKeyName);
            dbConn.ExecuteSql(dropSql);
        }

        /// <summary>
        /// Drops the constraint.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="field">The field.</param>
        public string GetConstraint<T>(string field)
        {
            var provider = dbConn.GetDialectProvider();
            var modelDef = ModelDefinition<T>.Definition;

            //var fieldDef = modelDef.GetFieldDefinition(field);
            //var fieldName = provider.NamingStrategy.GetColumnName(fieldDef.FieldName);

            var fieldName = provider.NamingStrategy.GetColumnName(field);

            var command = provider.GetConstraintName(dbConn.Database, modelDef, fieldName);

            try
            {
                return dbConn.SqlScalar<string>(command);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Drops the constraint.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="name">The name.</param>
        public void DropConstraint<T>(string name)
        {
            var provider = dbConn.GetDialectProvider();
            var modelDef = ModelDefinition<T>.Definition;

            var command = provider.GetDropConstraint(modelDef, name);

            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Creates the index.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="unique">if set to <c>true</c> [unique].</param>
        public void CreateIndex<T>(Expression<Func<T, object>> field,
            string indexName = null,
            bool unique = false)
        {
            var command = dbConn.GetDialectProvider().ToCreateIndexStatement(field, indexName, unique);
            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Drop Index of table
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="name">The name.</param>
        public void DropIndex<T>(string name = null)
        {
            var provider = dbConn.GetDialectProvider();
            var modelDef = ModelDefinition<T>.Definition;

            var command = provider.GetDropIndexConstraint(modelDef, name);

            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Creates the index of the view.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="selectSql">The select SQL.</param>
        public void CreateViewIndex<T>(string name, string selectSql)
        {
            var provider = dbConn.GetDialectProvider();
            var modelDef = ModelDefinition<T>.Definition;

            var command = provider.GetCreateIndexView(modelDef, name, selectSql);

            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Drops the index of the view.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="name">The name.</param>
        public void DropViewIndex<T>(string name)
        {
            var provider = dbConn.GetDialectProvider();
            var modelDef = ModelDefinition<T>.Definition;

            var command = provider.GetDropIndexView(modelDef, name);

            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Creates the view.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="selectSql">The select SQL.</param>
        public void CreateView<T>(StringBuilder selectSql)
        {
            var provider = dbConn.GetDialectProvider();
            var modelDef = ModelDefinition<T>.Definition;

            var command = provider.GetCreateView(dbConn.Database, modelDef, selectSql);

            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Drops the view.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        public void DropView<T>()
        {
            var provider = dbConn.GetDialectProvider();
            var modelDef = ModelDefinition<T>.Definition;

            var command = provider.GetDropView(dbConn.Database, modelDef);

            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Drops the function.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        public void DropFunction(string functionName)
        {
            var provider = dbConn.GetDialectProvider();

            var command = provider.GetDropFunction(dbConn.Database, functionName);

            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Alter tables by adding properties for missing columns and removing properties annotated with [RemoveColumn]
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <exception cref="Exception">$"Only 1 AlterColumnAttribute allowed on {modelType.Name}.{fieldDef.Name}</exception>
        /// <exception cref="Exception">$"Unsupported AlterColumnAttribute '{attr.GetType().Name}' on {modelType.Name}.{fieldDef.Name}</exception>
        public void Migrate(Type modelType)
        {
            var modelDef = modelType.GetModelDefinition();
            var migrateFieldDefinitions = modelDef.FieldDefinitions.Map(x => x.Clone(f => {
                f.IsPrimaryKey = false;
            }));
            foreach (var fieldDef in migrateFieldDefinitions)
            {
                var attrs = fieldDef.PropertyInfo.AllAttributes().Where(x => x is AlterColumnAttribute).ToList();
                if (attrs.Count > 1)
                {
                    throw new Exception($"Only 1 AlterColumnAttribute allowed on {modelType.Name}.{fieldDef.Name}");
                }

                var attr = attrs.FirstOrDefault();

                switch (attr)
                {
                    case RemoveColumnAttribute:
                        if (fieldDef.DefaultValueConstraint != null)
                        {
                            dbConn.DropConstraint(modelType, fieldDef.DefaultValueConstraint);
                        }
                        dbConn.DropColumn(modelType, fieldDef.FieldName);
                        break;
                    case RenameColumnAttribute renameAttr:
                        dbConn.RenameColumn(modelType, renameAttr.From, fieldDef.FieldName);
                        break;
                    case AddColumnAttribute or null:
                        dbConn.AddColumn(modelType, fieldDef);
                        break;
                    default:
                        throw new Exception($"Unsupported AlterColumnAttribute '{attr.GetType().Name}' on {modelType.Name}.{fieldDef.Name}");
                }
            }
        }

        /// <summary>
        /// Apply schema changes by Migrate in reverse to revert changes
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <exception cref="Exception">$"Only 1 AlterColumnAttribute allowed on {modelType.Name}.{fieldDef.Name}</exception>
        /// <exception cref="Exception">$"Unsupported AlterColumnAttribute '{attr.GetType().Name}' on {modelType.Name}.{fieldDef.Name}</exception>
        public void Revert(Type modelType)
        {
            var modelDef = modelType.GetModelDefinition();
            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                var attrs = fieldDef.PropertyInfo.AllAttributes().Where(x => x is AlterColumnAttribute).ToList();
                if (attrs.Count > 1)
                {
                    throw new Exception($"Only 1 AlterColumnAttribute allowed on {modelType.Name}.{fieldDef.Name}");
                }

                var attr = attrs.FirstOrDefault();
                switch (attr)
                {
                    case AddColumnAttribute or null:
                        if (fieldDef.DefaultValueConstraint != null)
                        {
                            dbConn.DropConstraint(modelType, fieldDef.DefaultValueConstraint);
                        }
                        dbConn.DropColumn(modelType, fieldDef.FieldName);
                        break;
                    case RenameColumnAttribute renameAttr:
                        dbConn.RenameColumn(modelType, fieldDef.FieldName, renameAttr.From);
                        break;
                    case RemoveColumnAttribute:
                        dbConn.AddColumn(modelType, fieldDef);
                        break;
                    default:
                        throw new Exception($"Unsupported AlterColumnAttribute '{attr.GetType().Name}' on {modelType.Name}.{fieldDef.Name}");
                }
            }
        }
    }
}