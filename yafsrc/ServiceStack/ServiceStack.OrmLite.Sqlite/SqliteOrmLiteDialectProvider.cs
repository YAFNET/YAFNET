﻿// ***********************************************************************
// <copyright file="SqliteOrmLiteDialectProvider.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.OrmLite.Sqlite;

using System;
using System.Data;
using System.Data.SQLite;

/// <summary>
/// Class SqliteOrmLiteDialectProvider.
/// Implements the <see cref="ServiceStack.OrmLite.Sqlite.SqliteOrmLiteDialectProviderBase" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Sqlite.SqliteOrmLiteDialectProviderBase" />
public class SqliteOrmLiteDialectProvider : SqliteOrmLiteDialectProviderBase
{
    /// <summary>
    /// The instance
    /// </summary>
    public static SqliteOrmLiteDialectProvider Instance = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteOrmLiteDialectProvider"/> class.
    /// </summary>
    public SqliteOrmLiteDialectProvider()
    {
#if NETFX
        ConnectionStringFilter = sb =>
        {
            if (sb.ToString().IndexOf("Version=3", StringComparison.OrdinalIgnoreCase) == -1)
            {
                sb.Append("Version=3;New=True;Compress=True");
            }
        };
#endif
    }

    /// <summary>
    /// Creates the connection.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>IDbConnection.</returns>
    override protected IDbConnection CreateConnection(string connectionString)
    {
        return new SQLiteConnection(connectionString);
    }

    /// <summary>
    /// Creates the parameter.
    /// </summary>
    /// <returns>IDbDataParameter.</returns>
    public override IDbDataParameter CreateParam()
    {
        return new SQLiteParameter();
    }
}