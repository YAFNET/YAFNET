// ***********************************************************************
// <copyright file="OrmLiteDialectProviderExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;

using ServiceStack.OrmLite.Base.Text;
using ServiceStack.OrmLite.Converters;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteDialectProviderExtensions.
/// </summary>
public static class OrmLiteDialectProviderExtensions
{
    /// <param name="dialect">The dialect.</param>
    extension(IOrmLiteDialectProvider dialect)
    {
        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="format">The format.</param>
        /// <returns>System.String.</returns>
        public string GetParam(string name, string format)
        {
            var ret = dialect.ParamString + (dialect.ParamNameFilter?.Invoke(name) ?? name);
            return format == null
                ? ret
                : string.Format(format, ret);
        }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public string GetParam(string name)
        {
            return dialect.ParamString + (dialect.ParamNameFilter?.Invoke(name) ?? name);
        }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <param name="indexNo">The index no.</param>
        /// <returns>System.String.</returns>
        public string GetParam(int indexNo = 0)
        {
            return dialect.ParamString + indexNo;
        }

        /// <summary>
        /// Converts to fieldname.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>System.String.</returns>
        public string ToFieldName(string paramName)
        {
            return paramName[dialect.ParamString.Length..];
        }
    }

    /// <param name="tableName">Name of the table.</param>
    extension(string tableName)
    {
        /// <summary>
        /// FMTs the table.
        /// </summary>
        /// <param name="dialect">The dialect.</param>
        /// <returns>System.String.</returns>
        public string FmtTable(IOrmLiteDialectProvider dialect = null)
        {
            return (dialect ?? OrmLiteConfig.DialectProvider).NamingStrategy.GetTableName(tableName);
        }

        /// <summary>
        /// FMTs the column.
        /// </summary>
        /// <param name="dialect">The dialect.</param>
        /// <returns>System.String.</returns>
        public string FmtColumn(IOrmLiteDialectProvider dialect = null)
        {
            return (dialect ?? OrmLiteConfig.DialectProvider).NamingStrategy.GetColumnName(tableName);
        }
    }

    /// <param name="dialect">The dialect.</param>
    extension(IOrmLiteDialectProvider dialect)
    {
        public string GetQuotedTableName(string tableName)
        {
            return tableName == null ? null : dialect.QuoteTable(new TableRef(tableName));
        }

        public string GetTableName(string tableName)
        {
            return tableName == null ? null : dialect.UnquotedTable(new TableRef(tableName));
        }

        public string GetTableName(Type table)
        {
            return table == null ? null : dialect.UnquotedTable(new TableRef(table.GetModelDefinition()));
        }

        public string GetTableName(ModelDefinition modelDef)
        {
            return modelDef == null ? null : dialect.UnquotedTable(new TableRef(modelDef));
        }

        /// <summary>
        /// Gets the name of the quoted column.
        /// </summary>
        /// <param name="tableDef">The table definition.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.String.</returns>
        public string GetQuotedColumnName(ModelDefinition tableDef, FieldDefinition fieldDef)
        {
            return dialect.GetQuotedTableName(tableDef) +
                   "." +
                   dialect.GetQuotedColumnName(fieldDef);
        }

        /// <summary>
        /// Gets the name of the quoted column.
        /// </summary>
        /// <param name="tableDef">The table definition.</param>
        /// <param name="tableAlias">The table alias.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.String.</returns>
        public string GetQuotedColumnName(ModelDefinition tableDef, string tableAlias, FieldDefinition fieldDef)
        {
            if (tableAlias == null)
            {
                return dialect.GetQuotedColumnName(tableDef, fieldDef);
            }

            return dialect.QuoteTable(tableAlias) //aliases shouldn't have schemas
                   + "." +
                   dialect.GetQuotedColumnName(fieldDef);
        }

        /// <summary>
        /// Gets the name of the quoted column.
        /// </summary>
        /// <param name="tableDef">The table definition.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>System.String.</returns>
        public string GetQuotedColumnName(ModelDefinition tableDef, string fieldName)
        {
            return dialect.GetQuotedTableName(tableDef) +
                   "." +
                   dialect.GetQuotedColumnName(fieldName);
        }

        /// <summary>
        /// Gets the name of the quoted column.
        /// </summary>
        /// <param name="tableDef">The table definition.</param>
        /// <param name="tableAlias">The table alias.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>System.String.</returns>
        public string GetQuotedColumnName(ModelDefinition tableDef, string tableAlias, string fieldName)
        {
            if (tableAlias == null)
            {
                return dialect.GetQuotedColumnName(tableDef, fieldName);
            }

            return dialect.QuoteTable(tableAlias) //aliases shouldn't have schemas
                   + "." +
                   dialect.GetQuotedColumnName(fieldName);
        }

        /// <summary>
        /// Froms the database value.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public object FromDbValue(IDataReader reader, int columnIndex, Type type)
        {
            return dialect.FromDbValue(dialect.GetValue(reader, columnIndex, type), type);
        }

        /// <summary>
        /// Gets the converter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IOrmLiteConverter.</returns>
        public IOrmLiteConverter GetConverter<T>()
        {
            return dialect.GetConverter(typeof(T));
        }

        /// <summary>
        /// Determines whether the specified type has converter.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type has converter; otherwise, <c>false</c>.</returns>
        public bool HasConverter(Type type)
        {
            return dialect.GetConverter(type) != null;
        }

        /// <summary>
        /// Gets the string converter.
        /// </summary>
        /// <returns>StringConverter.</returns>
        public StringConverter GetStringConverter()
        {
            return (StringConverter)dialect.GetConverter(typeof(string));
        }

        /// <summary>
        /// Gets the decimal converter.
        /// </summary>
        /// <returns>DecimalConverter.</returns>
        public DecimalConverter GetDecimalConverter()
        {
            return (DecimalConverter)dialect.GetConverter(typeof(decimal));
        }

        /// <summary>
        /// Gets the date time converter.
        /// </summary>
        /// <returns>DateTimeConverter.</returns>
        public DateTimeConverter GetDateTimeConverter()
        {
            return (DateTimeConverter)dialect.GetConverter(typeof(DateTime));
        }

        /// <summary>
        /// Determines whether [is my SQL connector] [the specified dialect].
        /// </summary>
        /// <returns><c>true</c> if [is my SQL connector] [the specified dialect]; otherwise, <c>false</c>.</returns>
        public bool IsMySqlConnector()
        {
            return dialect.GetType().Name == "MySqlConnectorDialectProvider";
        }

        /// <summary>
        /// Initializes the database parameter.
        /// </summary>
        /// <param name="dbParam">The database parameter.</param>
        /// <param name="columnType">Type of the column.</param>
        public void InitDbParam(IDbDataParameter dbParam, Type columnType)
        {
            var converter = dialect.GetConverterBestMatch(columnType);
            converter.InitDbParam(dbParam, columnType);
        }

        /// <summary>
        /// Initializes the database parameter.
        /// </summary>
        /// <param name="dbParam">The database parameter.</param>
        /// <param name="columnType">Type of the column.</param>
        /// <param name="value">The value.</param>
        public void InitDbParam(IDbDataParameter dbParam, Type columnType, object value)
        {
            var converter = dialect.GetConverterBestMatch(columnType);
            converter.InitDbParam(dbParam, columnType);
            dbParam.Value = converter.ToDbValue(columnType, value);
        }

        /// <summary>
        /// SQLs the spread.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <returns>System.String.</returns>
        public string SqlSpread<T>(params T[] values)
        {
            return OrmLiteUtils.SqlJoin(values, dialect);
        }

        /// <summary>
        /// Converts to addcolumnstatement.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.String.</returns>
        public string ToAddColumnStatement(Type modelType, FieldDefinition fieldDef)
        {
            return X.Map(modelType.GetModelDefinition(),
                x => dialect.ToAddColumnStatement(new TableRef(x), fieldDef));
        }

        /// <summary>
        /// Converts to altercolumnstatement.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.String.</returns>
        public string ToAlterColumnStatement(Type modelType, FieldDefinition fieldDef)
        {
            return X.Map(modelType.GetModelDefinition(),
                x => dialect.ToAlterColumnStatement(new TableRef(x), fieldDef));
        }

        /// <summary>
        /// Converts to changecolumnnamestatement.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <param name="oldColumnName">Old name of the column.</param>
        /// <returns>System.String.</returns>
        public string ToChangeColumnNameStatement(Type modelType, FieldDefinition fieldDef, string oldColumnName)
        {
            return X.Map(modelType.GetModelDefinition(),
                x => dialect.ToChangeColumnNameStatement(new TableRef(x), fieldDef, oldColumnName));
        }

        /// <summary>
        /// Converts to renamecolumnstatement.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="oldColumnName">Old name of the column.</param>
        /// <param name="newColumnName">New name of the column.</param>
        /// <returns>System.String.</returns>
        public string ToRenameColumnStatement(Type modelType, string oldColumnName, string newColumnName)
        {
            return X.Map(modelType.GetModelDefinition(),
                x => dialect.ToRenameColumnStatement(new TableRef(x), oldColumnName, newColumnName));
        }

        /// <summary>
        /// Converts to dropcolumnstatement.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        public string ToDropColumnStatement(Type modelType, string columnName)
        {
            return X.Map(modelType.GetModelDefinition(),
                x => dialect.ToDropColumnStatement(new TableRef(x), columnName));
        }

        /// <summary>
        /// Converts to dropconstraintstatement.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="constraintName">Name of the constraint.</param>
        /// <returns>System.String.</returns>
        public string ToDropConstraintStatement(Type modelType, string constraintName)
        {
            return X.Map(modelType.GetModelDefinition(),
                x => dialect.ToDropConstraintStatement(new TableRef(x), constraintName));
        }
    }
}