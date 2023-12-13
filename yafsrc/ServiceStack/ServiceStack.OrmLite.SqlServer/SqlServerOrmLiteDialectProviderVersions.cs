// ***********************************************************************
// <copyright file="SqlServerOrmLiteDialectProviderVersions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.SqlServer.Converters;

namespace ServiceStack.OrmLite.SqlServer;

/// <summary>
/// Class SqlServer2016OrmLiteDialectProvider.
/// Implements the <see cref="ServiceStack.OrmLite.SqlServer.SqlServer2014OrmLiteDialectProvider" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.SqlServer.SqlServer2014OrmLiteDialectProvider" />
public class SqlServer2016OrmLiteDialectProvider : SqlServer2014OrmLiteDialectProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServer2016OrmLiteDialectProvider"/> class.
    /// </summary>
    public SqlServer2016OrmLiteDialectProvider() : base()
    {
        base.RegisterConverter<string>(new SqlServerJsonStringConverter());
    }

    /// <summary>
    /// The instance
    /// </summary>
    public static new SqlServer2016OrmLiteDialectProvider Instance = new();

    /// <summary>
    /// SQL expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public override SqlExpression<T> SqlExpression<T>() => new SqlServer2016Expression<T>(this);
}

/// <summary>
/// Class SqlServer2017OrmLiteDialectProvider.
/// Implements the <see cref="ServiceStack.OrmLite.SqlServer.SqlServer2016OrmLiteDialectProvider" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.SqlServer.SqlServer2016OrmLiteDialectProvider" />
public class SqlServer2017OrmLiteDialectProvider : SqlServer2016OrmLiteDialectProvider
{
    /// <summary>
    /// The instance
    /// </summary>
    public static new SqlServer2017OrmLiteDialectProvider Instance = new();
}

/// <summary>
/// Class SqlServer2019OrmLiteDialectProvider.
/// Implements the <see cref="ServiceStack.OrmLite.SqlServer.SqlServer2017OrmLiteDialectProvider" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.SqlServer.SqlServer2017OrmLiteDialectProvider" />
public class SqlServer2019OrmLiteDialectProvider : SqlServer2017OrmLiteDialectProvider
{
    /// <summary>
    /// The instance
    /// </summary>
    public static new SqlServer2019OrmLiteDialectProvider Instance = new();
}

/// <summary>
/// Class SqlServer2022OrmLiteDialectProvider.
/// Implements the <see cref="ServiceStack.OrmLite.SqlServer.SqlServer2019OrmLiteDialectProvider" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.SqlServer.SqlServer2019OrmLiteDialectProvider" />
public class SqlServer2022OrmLiteDialectProvider : SqlServer2019OrmLiteDialectProvider
{
    /// <summary>
    /// The instance
    /// </summary>
    public static new SqlServer2022OrmLiteDialectProvider Instance = new();
}