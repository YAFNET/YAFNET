// ***********************************************************************
// <copyright file="DbConnectionFactory.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
#if !SL5
using System;
using System.Data;

namespace ServiceStack.Data;

/// <summary>
/// Class DbConnectionFactory.
/// Implements the <see cref="ServiceStack.Data.IDbConnectionFactory" />
/// </summary>
/// <seealso cref="ServiceStack.Data.IDbConnectionFactory" />
public class DbConnectionFactory(Func<IDbConnection> connectionFactoryFn) : IDbConnectionFactory
{
    /// <summary>
    /// Opens the database connection.
    /// </summary>
    /// <returns>IDbConnection.</returns>
    public IDbConnection OpenDbConnection()
    {
        var dbConn = this.CreateDbConnection();
        dbConn.Open();
        return dbConn;
    }

    /// <summary>
    /// Creates the database connection.
    /// </summary>
    /// <returns>IDbConnection.</returns>
    public IDbConnection CreateDbConnection()
    {
        return connectionFactoryFn();
    }
}
#endif