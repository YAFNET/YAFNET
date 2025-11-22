// ***********************************************************************
// <copyright file="OrmLiteSchemaApi.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.Collections.Generic;

namespace ServiceStack.OrmLite;

using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Class OrmLiteSchemaApi.
/// </summary>
public static class OrmLiteSchemaApi
{
    /// <param name="dbConn">The database connection.</param>
    extension(IDbConnection dbConn)
    {
        /// <summary>
        /// Checks whether a Table Exists. E.g:
        /// </summary>
        /// <param name="tableRef">The table reference.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TableExists(TableRef tableRef)
        {
            return dbConn.GetDialectProvider().DoesTableExist(dbConn, tableRef);
        }

        /// <summary>
        /// Checks whether a Table Exists. E.g:
        /// <para>db.TableExistsAsync("Person")</para>
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> TableExistsAsync(TableRef tableRef, CancellationToken token = default)
        {
            return dbConn.GetDialectProvider().DoesTableExistAsync(dbConn, tableRef, token);
        }

        /// <summary>
        /// Checks whether a Table Exists. E.g:
        /// <para>db.TableExists&lt;Person&gt;()</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TableExists<T>()
        {
            var dialectProvider = dbConn.GetDialectProvider();
            var modelDef = typeof(T).GetModelDefinition();
            return dialectProvider.DoesTableExist(dbConn, new TableRef(modelDef));
        }

        /// <summary>
        /// Checks whether a Table Exists. E.g:
        /// <para>db.TableExistsAsync&lt;Person&gt;()</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> TableExistsAsync<T>(CancellationToken token = default)
        {
            var dialectProvider = dbConn.GetDialectProvider();
            var modelDef = typeof(T).GetModelDefinition();
            return dialectProvider.DoesTableExistAsync(dbConn, new TableRef(modelDef), token);
        }

        /// <summary>
        /// Checks whether a Table Column Exists. E.g:
        /// <para>db.ColumnExists("Age", "Person")</para>
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="tableRef">The table reference.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ColumnExists(string columnName, TableRef tableRef)
        {
            return dbConn.GetDialectProvider().DoesColumnExist(dbConn, columnName, tableRef);
        }

        /// <summary>
        /// Checks whether a Table Column Exists. E.g:
        /// <para>db.ColumnExistsAsync("Age", "Person")</para>
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ColumnExistsAsync(string columnName, TableRef tableRef, CancellationToken token = default)
        {
            return dbConn.GetDialectProvider().DoesColumnExistAsync(dbConn, columnName, tableRef, token);
        }

        /// <summary>
        /// Checks whether a Table Column Exists. E.g:
        /// <para>
        /// db.ColumnExists&lt;Person&gt;("Age")
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName">The column Name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ColumnExists<T>(string columnName)
        {
            var dialectProvider = dbConn.GetDialectProvider();
            var modelDef = typeof(T).GetModelDefinition();
            return dialectProvider.DoesColumnExist(dbConn, columnName, new TableRef(modelDef));
        }

        /// <summary>
        /// Checks whether a Table Column Exists. E.g:
        /// <para>db.ColumnExists&lt;Person&gt;(x =&gt; x.Age)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ColumnExists<T>(Expression<Func<T, object>> field)
        {
            var dialectProvider = dbConn.GetDialectProvider();
            var modelDef = typeof(T).GetModelDefinition();
            var fieldDef = modelDef.GetFieldDefinition(field);
            var fieldName = dialectProvider.NamingStrategy.GetColumnName(fieldDef.FieldName);
            return dialectProvider.DoesColumnExist(dbConn, fieldName, new TableRef(modelDef));
        }

        /// <summary>
        /// Checks whether a Table Column Exists. E.g:
        /// <para>db.ColumnExistsAsync&lt;Person&gt;(x =&gt; x.Age)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ColumnExistsAsync<T>(Expression<Func<T, object>> field, CancellationToken token = default)
        {
            var dialectProvider = dbConn.GetDialectProvider();
            var modelDef = typeof(T).GetModelDefinition();
            var fieldDef = modelDef.GetFieldDefinition(field);
            var fieldName = dialectProvider.NamingStrategy.GetColumnName(fieldDef.FieldName);
            return dialectProvider.DoesColumnExistAsync(dbConn, fieldName, new TableRef(modelDef), token);
        }

        /// <summary>
        /// Checks if The Column allows Null Values or not. E.g:
        /// <para>db.ColumnIsNullable&lt;Person&gt;(x =&gt; x.Age)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field.</param>
        /// <returns>System.String.</returns>
        public string ColumnDataType<T>(Expression<Func<T, object>> field)
        {
            var dialectProvider = dbConn.GetDialectProvider();
            var modelDef = typeof(T).GetModelDefinition();
            var fieldDef = modelDef.GetFieldDefinition(field);
            var fieldName = dialectProvider.NamingStrategy.GetColumnName(fieldDef.FieldName);
            return dialectProvider.GetColumnDataType(dbConn, fieldName, new TableRef(modelDef));
        }

        /// <summary>
        /// Checks if The Column allows Null Values or not. E.g:
        /// <para>db.ColumnIsNullable&lt;Person&gt;(x =&gt; x.Age)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ColumnIsNullable<T>(Expression<Func<T, object>> field)
        {
            var dialectProvider = dbConn.GetDialectProvider();
            var modelDef = typeof(T).GetModelDefinition();
            var fieldDef = modelDef.GetFieldDefinition(field);
            var fieldName = dialectProvider.NamingStrategy.GetColumnName(fieldDef.FieldName);
            return dialectProvider.ColumnIsNullable(dbConn, fieldName, new TableRef(modelDef));
        }

        /// <summary>
        /// Gets the Max. Length for the Column. E.g:
        /// <para>db.ColumnExists&lt;Person&gt;(x =&gt; x.Age)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field.</param>
        /// <returns>System.Int64.</returns>
        public long ColumnMaxLength<T>(Expression<Func<T, object>> field)
        {
            var dialectProvider = dbConn.GetDialectProvider();
            var modelDef = typeof(T).GetModelDefinition();
            var fieldDef = modelDef.GetFieldDefinition(field);
            var fieldName = dialectProvider.NamingStrategy.GetColumnName(fieldDef.FieldName);
            return dialectProvider.GetColumnMaxLength(dbConn, fieldName, new TableRef(modelDef));
        }

        /// <summary>
        /// Create a DB Schema from the Schema attribute on the generic type. E.g:
        /// <para>db.CreateSchema&lt;Person&gt;() //default</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CreateSchema<T>()
        {
            dbConn.Exec(dbCmd => dbCmd.CreateSchema<T>());
        }

        /// <summary>
        /// Create a DB Schema. E.g:
        /// <para>db.CreateSchema("schemaName")</para>
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool CreateSchema(string schemaName)
        {
            return dbConn.Exec(dbCmd => dbCmd.CreateSchema(schemaName));
        }

        /// <summary>
        /// Create DB Tables from the schemas of runtime types. E.g:
        /// <para>db.CreateTables(typeof(Table1), typeof(Table2))</para>
        /// </summary>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <param name="tableTypes">The table types.</param>
        public void CreateTables(bool overwrite, params Type[] tableTypes)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTables(overwrite, tableTypes));
        }

        /// <summary>
        /// Create DB Table from the schema of the runtime type. Use overwrite to drop existing Table. E.g:
        /// <para>db.CreateTable(true, typeof(Table))</para>
        /// </summary>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <param name="modelType">Type of the model.</param>
        public void CreateTable(bool overwrite, Type modelType)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTable(overwrite, modelType));
        }

        /// <summary>
        /// Only Create new DB Tables from the schemas of runtime types if they don't already exist. E.g:
        /// <para>db.CreateTableIfNotExists(typeof(Table1), typeof(Table2))</para>
        /// </summary>
        /// <param name="tableTypes">The table types.</param>
        public void CreateTableIfNotExists(params Type[] tableTypes)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTables(overwrite: false, tableTypes: tableTypes));
        }

        /// <summary>
        /// Drop existing DB Tables and re-create them from the schemas of runtime types. E.g:
        /// <para>db.DropAndCreateTables(typeof(Table1), typeof(Table2))</para>
        /// </summary>
        /// <param name="tableTypes">The table types.</param>
        public void DropAndCreateTables(params Type[] tableTypes)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTables(overwrite: true, tableTypes: tableTypes));
        }

        /// <summary>
        /// Create a DB Table from the generic type. Use overwrite to drop the existing table or not. E.g:
        /// <para>db.CreateTable&lt;Person&gt;(overwrite=false) //default</para><para>db.CreateTable&lt;Person&gt;(overwrite=true)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        public void CreateTable<T>(bool overwrite = false)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTable<T>(overwrite));
        }

        /// <summary>
        /// Only create a DB Table from the generic type if it doesn't already exist. E.g:
        /// <para>db.CreateTableIfNotExists&lt;Person&gt;()</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool CreateTableIfNotExists<T>()
        {
            return dbConn.Exec(dbCmd => dbCmd.CreateTable<T>(overwrite: false));
        }

        /// <summary>
        /// Only create a DB Table from the runtime type if it doesn't already exist. E.g:
        /// <para>db.CreateTableIfNotExists(typeof(Person))</para>
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool CreateTableIfNotExists(Type modelType)
        {
            return dbConn.Exec(dbCmd => dbCmd.CreateTable(false, modelType));
        }

        /// <summary>
        /// Drop existing table if exists and re-create a DB Table from the generic type. E.g:
        /// <para>db.DropAndCreateTable&lt;Person&gt;()</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DropAndCreateTable<T>()
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTable<T>(true));
        }

        /// <summary>
        /// Drop existing table if exists and re-create a DB Table from the runtime type. E.g:
        /// <para>db.DropAndCreateTable(typeof(Person))</para>
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        public void DropAndCreateTable(Type modelType)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTable(true, modelType));
        }

        /// <summary>
        /// Drop any existing tables from their runtime types. E.g:
        /// <para>db.DropTables(typeof(Table1),typeof(Table2))</para>
        /// </summary>
        /// <param name="tableTypes">The table types.</param>
        public void DropTables(params Type[] tableTypes)
        {
            dbConn.Exec(dbCmd => dbCmd.DropTables(tableTypes));
        }

        /// <summary>
        /// Drop any existing tables from the runtime type. E.g:
        /// <para>db.DropTable("Person")</para>
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        public void DropTable(string tableName)
        {
            dbConn.Exec(dbCmd => dbCmd.DropTable(tableName));
        }

        /// <summary>
        /// Drop any existing tables from the runtime type. E.g:
        /// <para>db.DropTable(typeof(Person))</para>
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        public void DropTable(Type modelType)
        {
            dbConn.Exec(dbCmd => dbCmd.DropTable(modelType));
        }

        /// <summary>
        /// Drop any existing tables from the generic type. E.g:
        /// <para>db.DropTable&lt;Person&gt;()</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DropTable<T>()
        {
            dbConn.Exec(dbCmd => dbCmd.DropTable<T>());
        }

        /// <summary>
        /// Get a list of available user schemas for this connection
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        public List<string> GetSchemas()
        {
            return dbConn.Exec(dbCmd => dbConn.GetDialectProvider().GetSchemas(dbCmd));
        }

        /// <summary>
        /// Get available user Schemas and their tables for this connection
        /// </summary>
        /// <returns>Dictionary&lt;System.String, List&lt;System.String&gt;&gt;.</returns>
        public Dictionary<string, List<string>> GetSchemaTables()
        {
            return dbConn.Exec(dbCmd => dbConn.GetDialectProvider().GetSchemaTables(dbCmd));
        }

        /// <summary>
        /// Alter tables by adding properties for missing columns and removing properties annotated with [RemoveColumn]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Migrate<T>()
        {
            dbConn.Migrate(typeof(T));
        }

        /// <summary>
        /// Apply schema changes by Migrate in reverse to revert changes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Revert<T>()
        {
            dbConn.Revert(typeof(T));
        }
    }
}