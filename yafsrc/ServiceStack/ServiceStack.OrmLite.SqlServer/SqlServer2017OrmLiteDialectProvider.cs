// ***********************************************************************
// <copyright file="SqlServer2017OrmLiteDialectProvider.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.OrmLite.SqlServer;

/// <summary>
/// Class SqlServer2017OrmLiteDialectProvider.
/// Implements the <see cref="SqlServer2016OrmLiteDialectProvider" />
/// </summary>
/// <seealso cref="SqlServer2016OrmLiteDialectProvider" />
public class SqlServer2017OrmLiteDialectProvider : SqlServer2016OrmLiteDialectProvider
{
    /// <summary>
    /// The instance
    /// </summary>
    public static new SqlServer2017OrmLiteDialectProvider Instance = new();
}