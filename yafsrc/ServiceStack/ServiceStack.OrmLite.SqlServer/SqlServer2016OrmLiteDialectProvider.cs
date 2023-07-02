// ***********************************************************************
// <copyright file="SqlServer2016OrmLiteDialectProvider.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.OrmLite.SqlServer;

using System;

using ServiceStack.OrmLite.SqlServer.Converters;

/// <summary>
/// Class SqlServer2016OrmLiteDialectProvider.
/// Implements the <see cref="ServiceStack.OrmLite.SqlServer.SqlServer2014OrmLiteDialectProvider" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.SqlServer.SqlServer2014OrmLiteDialectProvider" />
public class SqlServer2016OrmLiteDialectProvider : SqlServer2014OrmLiteDialectProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServer2016OrmLiteDialectProvider" /> class.
    /// </summary>
    public SqlServer2016OrmLiteDialectProvider()
    {
        base.RegisterConverter<String>(new SqlServerJsonStringConverter());
    }

    /// <summary>
    /// The instance
    /// </summary>
    public static new SqlServer2016OrmLiteDialectProvider Instance = new();

    /// <summary>
    /// SQLs the expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public override SqlExpression<T> SqlExpression<T>() => new SqlServer2016Expression<T>(this);
}