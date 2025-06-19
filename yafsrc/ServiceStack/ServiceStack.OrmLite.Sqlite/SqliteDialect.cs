// ***********************************************************************
// <copyright file="SqliteDialect.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite;

using ServiceStack.OrmLite.Sqlite;

/// <summary>
/// Class SqliteDialect.
/// </summary>
public static class SqliteDialect
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <value>The provider.</value>
    public static IOrmLiteDialectProvider Provider => SqliteOrmLiteDialectProvider.Instance;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static SqliteOrmLiteDialectProvider Instance => SqliteOrmLiteDialectProvider.Instance;

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns>SqliteOrmLiteDialectProviderBase.</returns>
    public static SqliteOrmLiteDialectProviderBase Create() => new SqliteOrmLiteDialectProvider();
}