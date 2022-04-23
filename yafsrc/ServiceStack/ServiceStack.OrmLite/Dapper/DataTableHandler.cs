// ***********************************************************************
// <copyright file="DataTableHandler.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;
namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Class DataTableHandler. This class cannot be inherited.
/// Implements the <see cref="ServiceStack.OrmLite.Dapper.SqlMapper.ITypeHandler" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Dapper.SqlMapper.ITypeHandler" />
internal sealed class DataTableHandler : SqlMapper.ITypeHandler
{
    /// <summary>
    /// Parse a database value back to a typed value
    /// </summary>
    /// <param name="destinationType">The type to parse to</param>
    /// <param name="value">The value from the database</param>
    /// <returns>The typed value</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public object Parse(Type destinationType, object value)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <param name="value">The value.</param>
    public void SetValue(IDbDataParameter parameter, object value)
    {
        TableValuedParameter.Set(parameter, value as DataTable, null);
    }
}