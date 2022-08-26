// ***********************************************************************
// <copyright file="OrmLiteDialectProviderExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;
using ServiceStack.OrmLite.Converters;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteDialectProviderExtensions.
/// </summary>
public static class OrmLiteDialectProviderExtensions
{
    /// <summary>
    /// Gets the parameter.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <param name="name">The name.</param>
    /// <param name="format">The format.</param>
    /// <returns>System.String.</returns>
    public static string GetParam(this IOrmLiteDialectProvider dialect, string name, string format)
    {
        var ret = dialect.ParamString + (dialect.ParamNameFilter?.Invoke(name) ?? name);
        return format == null
                   ? ret
                   : string.Format(format, ret);
    }

    /// <summary>
    /// Gets the parameter.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public static string GetParam(this IOrmLiteDialectProvider dialect, string name)
    {
        return dialect.ParamString + (dialect.ParamNameFilter?.Invoke(name) ?? name);
    }

    /// <summary>
    /// Gets the parameter.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <param name="indexNo">The index no.</param>
    /// <returns>System.String.</returns>
    public static string GetParam(this IOrmLiteDialectProvider dialect, int indexNo = 0)
    {
        return dialect.ParamString + indexNo;
    }

    /// <summary>
    /// Converts to fieldname.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <param name="paramName">Name of the parameter.</param>
    /// <returns>System.String.</returns>
    public static string ToFieldName(this IOrmLiteDialectProvider dialect, string paramName)
    {
        return paramName.Substring(dialect.ParamString.Length);
    }

    /// <summary>
    /// FMTs the table.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>System.String.</returns>
    public static string FmtTable(this string tableName, IOrmLiteDialectProvider dialect = null)
    {
        return (dialect ?? OrmLiteConfig.DialectProvider).NamingStrategy.GetTableName(tableName);
    }

    /// <summary>
    /// FMTs the column.
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>System.String.</returns>
    public static string FmtColumn(this string columnName, IOrmLiteDialectProvider dialect = null)
    {
        return (dialect ?? OrmLiteConfig.DialectProvider).NamingStrategy.GetColumnName(columnName);
    }

    /// <summary>
    /// Gets the name of the quoted column.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    public static string GetQuotedColumnName(this IOrmLiteDialectProvider dialect,
                                             FieldDefinition fieldDef)
    {
        return dialect.GetQuotedColumnName(fieldDef.FieldName);
    }

    /// <summary>
    /// Gets the name of the quoted column.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <param name="tableDef">The table definition.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    public static string GetQuotedColumnName(this IOrmLiteDialectProvider dialect,
                                             ModelDefinition tableDef, FieldDefinition fieldDef)
    {
        return dialect.GetQuotedTableName(tableDef) +
               "." +
               dialect.GetQuotedColumnName(fieldDef.FieldName);
    }

    /// <summary>
    /// Gets the name of the quoted column.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <param name="tableDef">The table definition.</param>
    /// <param name="tableAlias">The table alias.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    public static string GetQuotedColumnName(this IOrmLiteDialectProvider dialect,
                                             ModelDefinition tableDef, string tableAlias, FieldDefinition fieldDef)
    {
        if (tableAlias == null)
            return dialect.GetQuotedColumnName(tableDef, fieldDef);

        return dialect.GetQuotedTableName(tableAlias) //aliases shouldn't have schemas
               + "." +
               dialect.GetQuotedColumnName(fieldDef.FieldName);
    }

    /// <summary>
    /// Gets the name of the quoted column.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <param name="tableDef">The table definition.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>System.String.</returns>
    public static string GetQuotedColumnName(this IOrmLiteDialectProvider dialect,
                                             ModelDefinition tableDef, string fieldName)
    {
        return dialect.GetQuotedTableName(tableDef) +
               "." +
               dialect.GetQuotedColumnName(fieldName);
    }

    /// <summary>
    /// Gets the name of the quoted column.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <param name="tableDef">The table definition.</param>
    /// <param name="tableAlias">The table alias.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>System.String.</returns>
    public static string GetQuotedColumnName(this IOrmLiteDialectProvider dialect,
                                             ModelDefinition tableDef, string tableAlias, string fieldName)
    {
        if (tableAlias == null)
            return dialect.GetQuotedColumnName(tableDef, fieldName);

        return dialect.GetQuotedTableName(tableAlias) //aliases shouldn't have schemas 
               + "." +
               dialect.GetQuotedColumnName(fieldName);
    }

    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <param name="reader">The reader.</param>
    /// <param name="columnIndex">Index of the column.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public static object FromDbValue(this IOrmLiteDialectProvider dialect,
                                     IDataReader reader, int columnIndex, Type type)
    {
        return dialect.FromDbValue(dialect.GetValue(reader, columnIndex, type), type);
    }

    /// <summary>
    /// Gets the converter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dialect">The dialect.</param>
    /// <returns>IOrmLiteConverter.</returns>
    public static IOrmLiteConverter GetConverter<T>(this IOrmLiteDialectProvider dialect)
    {
        return dialect.GetConverter(typeof(T));
    }

    /// <summary>
    /// Determines whether the specified type has converter.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if the specified type has converter; otherwise, <c>false</c>.</returns>
    public static bool HasConverter(this IOrmLiteDialectProvider dialect, Type type)
    {
        return dialect.GetConverter(type) != null;
    }

    /// <summary>
    /// Gets the string converter.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <returns>StringConverter.</returns>
    public static StringConverter GetStringConverter(this IOrmLiteDialectProvider dialect)
    {
        return (StringConverter)dialect.GetConverter(typeof(string));
    }

    /// <summary>
    /// Gets the decimal converter.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <returns>DecimalConverter.</returns>
    public static DecimalConverter GetDecimalConverter(this IOrmLiteDialectProvider dialect)
    {
        return (DecimalConverter)dialect.GetConverter(typeof(decimal));
    }

    /// <summary>
    /// Gets the date time converter.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <returns>DateTimeConverter.</returns>
    public static DateTimeConverter GetDateTimeConverter(this IOrmLiteDialectProvider dialect)
    {
        return (DateTimeConverter)dialect.GetConverter(typeof(DateTime));
    }

    /// <summary>
    /// Determines whether [is my SQL connector] [the specified dialect].
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <returns><c>true</c> if [is my SQL connector] [the specified dialect]; otherwise, <c>false</c>.</returns>
    public static bool IsMySqlConnector(this IOrmLiteDialectProvider dialect) =>
        dialect.GetType().Name == "MySqlConnectorDialectProvider";

    /// <summary>
    /// Initializes the database parameter.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <param name="dbParam">The database parameter.</param>
    /// <param name="columnType">Type of the column.</param>
    public static void InitDbParam(this IOrmLiteDialectProvider dialect, IDbDataParameter dbParam, Type columnType)
    {
        var converter = dialect.GetConverterBestMatch(columnType);
        converter.InitDbParam(dbParam, columnType);
    }

    /// <summary>
    /// Initializes the database parameter.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <param name="dbParam">The database parameter.</param>
    /// <param name="columnType">Type of the column.</param>
    /// <param name="value">The value.</param>
    public static void InitDbParam(this IOrmLiteDialectProvider dialect, IDbDataParameter dbParam, Type columnType, object value)
    {
        var converter = dialect.GetConverterBestMatch(columnType);
        converter.InitDbParam(dbParam, columnType);
        dbParam.Value = converter.ToDbValue(columnType, value);
    }

    /// <summary>
    /// SQLs the spread.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dialect">The dialect.</param>
    /// <param name="values">The values.</param>
    /// <returns>System.String.</returns>
    public static string SqlSpread<T>(this IOrmLiteDialectProvider dialect, params T[] values) =>
        OrmLiteUtils.SqlJoin(values, dialect);

    public static string ToAddColumnStatement(this IOrmLiteDialectProvider dialect, Type modelType, FieldDefinition fieldDef) =>
        X.Map(modelType.GetModelDefinition(), x => dialect.ToAddColumnStatement(x.Schema, x.ModelName, fieldDef));

    public static string ToAlterColumnStatement(this IOrmLiteDialectProvider dialect, Type modelType, FieldDefinition fieldDef) =>
        X.Map(modelType.GetModelDefinition(), x => dialect.ToAlterColumnStatement(x.Schema, x.ModelName, fieldDef));

    public static string ToChangeColumnNameStatement(this IOrmLiteDialectProvider dialect, Type modelType, FieldDefinition fieldDef, string oldColumnName) =>
        X.Map(modelType.GetModelDefinition(), x => dialect.ToChangeColumnNameStatement(x.Schema, x.ModelName, fieldDef, oldColumnName));

    public static string ToRenameColumnStatement(this IOrmLiteDialectProvider dialect, Type modelType, string oldColumnName, string newColumnName) =>
        X.Map(modelType.GetModelDefinition(), x => dialect.ToRenameColumnStatement(x.Schema, x.ModelName, oldColumnName, newColumnName));
    public static string ToDropColumnStatement(this IOrmLiteDialectProvider dialect, Type modelType, string columnName) =>
        X.Map(modelType.GetModelDefinition(), x => dialect.ToDropColumnStatement(x.Schema, x.ModelName, columnName));

}