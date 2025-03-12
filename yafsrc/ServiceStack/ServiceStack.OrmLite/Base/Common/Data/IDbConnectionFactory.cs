// ***********************************************************************
// <copyright file="IDbConnectionFactory.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
#if !SL5
using System.Data;

namespace ServiceStack.Data;

/// <summary>
/// Interface IDbConnectionFactory
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Opens the database connection.
    /// </summary>
    /// <returns>IDbConnection.</returns>
    IDbConnection OpenDbConnection();
    /// <summary>
    /// Creates the database connection.
    /// </summary>
    /// <returns>IDbConnection.</returns>
    IDbConnection CreateDbConnection();
}

/// <summary>
/// Interface IDbConnectionFactoryExtended
/// Implements the <see cref="ServiceStack.Data.IDbConnectionFactory" />
/// </summary>
/// <seealso cref="ServiceStack.Data.IDbConnectionFactory" />
public interface IDbConnectionFactoryExtended : IDbConnectionFactory
{
    /// <summary>
    /// Opens the database connection.
    /// </summary>
    /// <param name="namedConnection">The named connection.</param>
    /// <returns>IDbConnection.</returns>
    IDbConnection OpenDbConnection(string namedConnection);

    /// <summary>
    /// Opens the database connection string.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>IDbConnection.</returns>
    IDbConnection OpenDbConnectionString(string connectionString);
    /// <summary>
    /// Opens the database connection string.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="providerName">Name of the provider.</param>
    /// <returns>IDbConnection.</returns>
    IDbConnection OpenDbConnectionString(string connectionString, string providerName);

    /// <summary>
    /// Uses the specified connection.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="trans">The trans.</param>
    /// <returns>IDbConnection.</returns>
    IDbConnection Use(IDbConnection connection, IDbTransaction trans = null);
}
#endif
