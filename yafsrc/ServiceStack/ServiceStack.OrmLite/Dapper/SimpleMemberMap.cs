// ***********************************************************************
// <copyright file="SimpleMemberMap.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Reflection;

namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Represents simple member map for one of target parameter or property or field to source DataReader column
/// </summary>
internal sealed class SimpleMemberMap : SqlMapper.IMemberMap
{
    /// <summary>
    /// Creates instance for simple property mapping
    /// </summary>
    /// <param name="columnName">DataReader column name</param>
    /// <param name="property">Target property</param>
    /// <exception cref="System.ArgumentNullException">columnName</exception>
    /// <exception cref="System.ArgumentNullException">property</exception>
    public SimpleMemberMap(string columnName, PropertyInfo property)
    {
        ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
        Property = property ?? throw new ArgumentNullException(nameof(property));
    }

    /// <summary>
    /// Creates instance for simple field mapping
    /// </summary>
    /// <param name="columnName">DataReader column name</param>
    /// <param name="field">Target property</param>
    /// <exception cref="System.ArgumentNullException">columnName</exception>
    /// <exception cref="System.ArgumentNullException">field</exception>
    public SimpleMemberMap(string columnName, FieldInfo field)
    {
        ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
        Field = field ?? throw new ArgumentNullException(nameof(field));
    }

    /// <summary>
    /// Creates instance for simple constructor parameter mapping
    /// </summary>
    /// <param name="columnName">DataReader column name</param>
    /// <param name="parameter">Target constructor parameter</param>
    /// <exception cref="System.ArgumentNullException">columnName</exception>
    /// <exception cref="System.ArgumentNullException">parameter</exception>
    public SimpleMemberMap(string columnName, ParameterInfo parameter)
    {
        ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
        Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
    }

    /// <summary>
    /// DataReader column name
    /// </summary>
    /// <value>The name of the column.</value>
    public string ColumnName { get; }

    /// <summary>
    /// Target member type
    /// </summary>
    /// <value>The type of the member.</value>
    public Type MemberType => Field?.FieldType ?? Property?.PropertyType ?? Parameter?.ParameterType;

    /// <summary>
    /// Target property
    /// </summary>
    /// <value>The property.</value>
    public PropertyInfo Property { get; }

    /// <summary>
    /// Target field
    /// </summary>
    /// <value>The field.</value>
    public FieldInfo Field { get; }

    /// <summary>
    /// Target constructor parameter
    /// </summary>
    /// <value>The parameter.</value>
    public ParameterInfo Parameter { get; }
}