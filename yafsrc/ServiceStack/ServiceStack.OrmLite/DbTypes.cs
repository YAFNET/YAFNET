// ***********************************************************************
// <copyright file="DbTypes.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class DbTypes.
/// </summary>
/// <typeparam name="TDialect">The type of the t dialect.</typeparam>
public class DbTypes<TDialect>
    where TDialect : IOrmLiteDialectProvider
{
    /// <summary>
    /// The database type
    /// </summary>
    public DbType DbType;
    /// <summary>
    /// The text definition
    /// </summary>
    public string TextDefinition;
    /// <summary>
    /// The should quote value
    /// </summary>
    public bool ShouldQuoteValue;
    /// <summary>
    /// The column type map
    /// </summary>
    public Dictionary<Type, string> ColumnTypeMap = [];
    /// <summary>
    /// The column database type map
    /// </summary>
    public Dictionary<Type, DbType> ColumnDbTypeMap = [];

    /// <summary>
    /// Sets the specified database type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbType">Type of the database.</param>
    /// <param name="fieldDefinition">The field definition.</param>
    public void Set<T>(DbType dbType, string fieldDefinition)
    {
        this.DbType = dbType;
        this.TextDefinition = fieldDefinition;
        this.ShouldQuoteValue = fieldDefinition != "INTEGER"
                           && fieldDefinition != "BIGINT"
                           && fieldDefinition != "DOUBLE"
                           && fieldDefinition != "DECIMAL"
                           && fieldDefinition != "BOOL";

        this.ColumnTypeMap[typeof(T)] = fieldDefinition;
        this.ColumnDbTypeMap[typeof(T)] = dbType;
    }
}