// ***********************************************************************
// <copyright file="IHasDbConnection.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.Data;

namespace ServiceStack.Data;

/// <summary>
/// Interface IHasDbConnection
/// </summary>
public interface IHasDbConnection
{
    /// <summary>
    /// Gets the database connection.
    /// </summary>
    /// <value>The database connection.</value>
    IDbConnection DbConnection { get; }
}

/// <summary>
/// Interface IHasDbCommand
/// </summary>
public interface IHasDbCommand
{
    /// <summary>
    /// Gets the database command.
    /// </summary>
    /// <value>The database command.</value>
    IDbCommand DbCommand { get; }
}

/// <summary>
/// Interface IHasDbTransaction
/// </summary>
public interface IHasDbTransaction
{
    /// <summary>
    /// Gets the database transaction.
    /// </summary>
    /// <value>The database transaction.</value>
    IDbTransaction DbTransaction { get; }
}