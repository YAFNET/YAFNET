// ***********************************************************************
// <copyright file="OrmLiteSchemaModifyApi.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite
{
    using System;
    using System.Data;
    using System.Linq.Expressions;
    using System.Text;

    using ServiceStack.Text;

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

        /// <summary>
        /// Alters the table.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="command">The command.</param>
        public static void AlterTable<T>(this IDbConnection dbConn, string command)
        {
            AlterTable(dbConn, typeof(T), command);
        }

        /// <summary>
        /// Alters the table.
        /// </summary>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="command">The command.</param>
        public static void AlterTable(this IDbConnection dbConn, Type modelType, string command)
        {
            var sql = $"ALTER TABLE {dbConn.GetDialectProvider().GetQuotedTableName(modelType.GetModelDefinition())} {command};";
            dbConn.ExecuteSql(sql);
        }

        /// <summary>
        /// Adds the column with command.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="command">The command.</param>
        public static void AddColumnWithCommand<T>(this IDbConnection dbConn, string command)
        {
            var modelDef = ModelDefinition<T>.Definition;

            var sql = $"ALTER TABLE {dbConn.GetDialectProvider().GetQuotedTableName(modelDef)} ADD {command};";
            dbConn.ExecuteSql(sql);
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="field">The field.</param>
        public static void AddColumn<T>(this IDbConnection dbConn, Expression<Func<T, object>> field)
        {
            var modelDef = ModelDefinition<T>.Definition;
            var fieldDef = modelDef.GetFieldDefinition(field);
            dbConn.AddColumn(typeof(T), fieldDef);
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef">The field definition.</param>
        public static void AddColumn(this IDbConnection dbConn, Type modelType, FieldDefinition fieldDef)
        {
            InitUserFieldDefinition(modelType, fieldDef);

            var command = dbConn.GetDialectProvider().ToAddColumnStatement(modelType, fieldDef);
            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Alters the column.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="field">The field.</param>
        public static void AlterColumn<T>(this IDbConnection dbConn, Expression<Func<T, object>> field)
        {
            var modelDef = ModelDefinition<T>.Definition;
            var fieldDef = modelDef.GetFieldDefinition<T>(field);
            dbConn.AlterColumn(typeof(T), fieldDef);
        }

        /// <summary>
        /// Alters the column.
        /// </summary>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef">The field definition.</param>
        public static void AlterColumn(this IDbConnection dbConn, Type modelType, FieldDefinition fieldDef)
        {
            InitUserFieldDefinition(modelType, fieldDef);

            var command = dbConn.GetDialectProvider().ToAlterColumnStatement(modelType, fieldDef);
            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Changes the name of the column.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="field">The field.</param>
        /// <param name="oldColumnName">Old name of the column.</param>
        public static void ChangeColumnName<T>(this IDbConnection dbConn, Expression<Func<T, object>> field, string oldColumnName)
        {
            var modelDef = ModelDefinition<T>.Definition;
            var fieldDef = modelDef.GetFieldDefinition<T>(field);
            dbConn.ChangeColumnName(typeof(T), fieldDef, oldColumnName);
        }

        /// <summary>
        /// Changes the name of the column.
        /// </summary>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <param name="oldColumnName">Old name of the column.</param>
        public static void ChangeColumnName(
            this IDbConnection dbConn,
            Type modelType,
            FieldDefinition fieldDef,
            string oldColumnName)
        {
            var command = dbConn.GetDialectProvider().ToChangeColumnNameStatement(modelType, fieldDef, oldColumnName);
            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Drops the column.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="field">The field.</param>
        public static void DropColumn<T>(this IDbConnection dbConn, Expression<Func<T, object>> field)
        {
            var modelDef = ModelDefinition<T>.Definition;
            var fieldDef = modelDef.GetFieldDefinition(field);
            dbConn.DropColumn(typeof(T), fieldDef.FieldName);
        }

        /// <summary>
        /// Drops the column.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="columnName">Name of the column.</param>
        public static void DropColumn<T>(this IDbConnection dbConn, string columnName)
        {
            dbConn.DropColumn(typeof(T), columnName);
        }

        /// <summary>
        /// Drops the column.
        /// </summary>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="columnName">Name of the column.</param>
        public static void DropColumn(this IDbConnection dbConn, Type modelType, string columnName)
        {
            dbConn.GetDialectProvider().DropColumn(dbConn, modelType, columnName);
        }

        /// <summary>
        /// Adds the foreign key.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <typeparam name="TForeign">The type of the t foreign.</typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="field">The field.</param>
        /// <param name="foreignField">The foreign field.</param>
        /// <param name="onUpdate">The on update.</param>
        /// <param name="onDelete">The on delete.</param>
        /// <param name="foreignKeyName">Name of the foreign key.</param>
        public static void AddForeignKey<T, TForeign>(this IDbConnection dbConn,
            Expression<Func<T, object>> field,
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
        /// <typeparam name="T">
        /// The Table Model
        /// </typeparam>
        /// <param name="dbConn">
        /// The database connection.
        /// </param>
        /// <param name="fieldA">
        /// The field A.
        /// </param>
        /// <param name="fieldB">
        /// The field B.
        /// </param>
        public static void AddCompositePrimaryKey<T>(this IDbConnection dbConn, Expression<Func<T, object>> fieldA, Expression<Func<T, object>> fieldB)
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
        /// <typeparam name="T">
        /// The Model
        /// </typeparam>
        /// <param name="dbConn">
        /// The database connection.
        /// </param>
        /// <returns>
        /// Returns the key name
        /// </returns>
        public static string GetPrimaryKey<T>(this IDbConnection dbConn)
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
        /// <typeparam name="T">
        /// The Table Model
        /// </typeparam>
        /// <param name="dbConn">
        /// The database connection.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="fieldA">
        /// The field A.
        /// </param>
        /// <param name="fieldB">
        /// The field B.
        /// </param>
        public static void DropPrimaryKey<T>(this IDbConnection dbConn, string name, Expression<Func<T, object>> fieldA = null, Expression<Func<T, object>> fieldB = null)
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
        /// <param name="dbConn">The database connection.</param>
        /// <param name="name">The name.</param>
        public static void DropForeignKey<T>(this IDbConnection dbConn, string name)
        {
            var provider = dbConn.GetDialectProvider();
            var modelDef = ModelDefinition<T>.Definition;

            var command = provider.GetDropForeignKeyConstraint(modelDef, name);

            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Drops the constraint.
        /// </summary>
        /// <typeparam name="T">The Table Model</typeparam>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="name">The name.</param>
        public static void DropConstraint<T>(this IDbConnection dbConn, string name)
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
        /// <param name="dbConn">The database connection.</param>
        /// <param name="field">The field.</param>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="unique">if set to <c>true</c> [unique].</param>
        public static void CreateIndex<T>(
            this IDbConnection dbConn,
            Expression<Func<T, object>> field,
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
        /// <param name="dbConn">The db conn.</param>
        /// <param name="name">The name.</param>
        public static void DropIndex<T>(this IDbConnection dbConn, string name = null)
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
        /// <param name="dbConn">The database connection.</param>
        /// <param name="name">The name.</param>
        /// <param name="selectSql">The select SQL.</param>
        public static void CreateViewIndex<T>(this IDbConnection dbConn, string name, string selectSql)
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
        /// <param name="dbConn">The database connection.</param>
        /// <param name="name">The name.</param>
        public static void DropViewIndex<T>(this IDbConnection dbConn, string name)
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
        /// <param name="dbConn">The database connection.</param>
        /// <param name="selectSql">The select SQL.</param>
        public static void CreateView<T>(this IDbConnection dbConn, StringBuilder selectSql)
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
        /// <param name="dbConn">The database connection.</param>
        public static void DropView<T>(this IDbConnection dbConn)
        {
            var provider = dbConn.GetDialectProvider();
            var modelDef = ModelDefinition<T>.Definition;

            var command = provider.GetDropView(dbConn.Database, modelDef);

            dbConn.ExecuteSql(command);
        }

        /// <summary>
        /// Drops the function.
        /// </summary>
        /// <param name="dbConn">The database connection.</param>
        /// <param name="functionName">Name of the function.</param>
        public static void DropFunction(this IDbConnection dbConn, string functionName)
        {
            var provider = dbConn.GetDialectProvider();

            var command = provider.GetDropFunction(dbConn.Database, functionName);

            dbConn.ExecuteSql(command);
        }
    }
}
