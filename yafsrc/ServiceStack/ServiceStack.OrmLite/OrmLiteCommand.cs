// ***********************************************************************
// <copyright file="OrmLiteCommand.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Data;
using ServiceStack.Data;

namespace ServiceStack.OrmLite;

using System;

/// <summary>
/// Class OrmLiteCommand.
/// Implements the <see cref="System.Data.IDbCommand" />
/// Implements the <see cref="ServiceStack.Data.IHasDbCommand" />
/// Implements the <see cref="ServiceStack.OrmLite.IHasDialectProvider" />
/// </summary>
/// <seealso cref="System.Data.IDbCommand" />
/// <seealso cref="ServiceStack.Data.IHasDbCommand" />
/// <seealso cref="ServiceStack.OrmLite.IHasDialectProvider" />
public class OrmLiteCommand : IDbCommand, IHasDbCommand, IHasDialectProvider
{
    /// <summary>
    /// The database connection
    /// </summary>
    private readonly OrmLiteConnection dbConn;

    /// <summary>
    /// The database command
    /// </summary>
    private readonly IDbCommand dbCmd;
    /// <summary>
    /// Gets the dialect provider.
    /// </summary>
    /// <value>The dialect provider.</value>
    public IOrmLiteDialectProvider DialectProvider { get; set; }
    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrmLiteCommand" /> class.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="dbCmd">The database command.</param>
    public OrmLiteCommand(OrmLiteConnection dbConn, IDbCommand dbCmd)
    {
        this.dbConn = dbConn;
        this.dbCmd = dbCmd;
        this.DialectProvider = dbConn.GetDialectProvider();
    }

    /// <summary>
    /// Gets the connection identifier.
    /// </summary>
    /// <value>The connection identifier.</value>
    public Guid ConnectionId => dbConn.ConnectionId;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        IsDisposed = true;
        dbCmd.Dispose();
    }

    /// <summary>
    /// Creates a prepared (or compiled) version of the command on the data source.
    /// </summary>
    public void Prepare()
    {
        dbCmd.Prepare();
    }

    /// <summary>
    /// Attempts to cancels the execution of an <see cref="T:System.Data.IDbCommand" />.
    /// </summary>
    public void Cancel()
    {
        dbCmd.Cancel();
    }

    /// <summary>
    /// Creates a new instance of an <see cref="T:System.Data.IDbDataParameter" /> object.
    /// </summary>
    /// <returns>An <see langword="IDbDataParameter" /> object.</returns>
    public IDbDataParameter CreateParameter()
    {
        return dbCmd.CreateParameter();
    }

    /// <summary>
    /// Executes an SQL statement against the <see langword="Connection" /> object of a .NET Framework data provider, and returns the number of rows affected.
    /// </summary>
    /// <returns>The number of rows affected.</returns>
    public int ExecuteNonQuery()
    {
        return dbCmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Executes the <see cref="P:System.Data.IDbCommand.CommandText" /> against the <see cref="P:System.Data.IDbCommand.Connection" /> and builds an <see cref="T:System.Data.IDataReader" />.
    /// </summary>
    /// <returns>An <see cref="T:System.Data.IDataReader" /> object.</returns>
    public IDataReader ExecuteReader()
    {
        return dbCmd.ExecuteReader();
    }

    /// <summary>
    /// Executes the <see cref="P:System.Data.IDbCommand.CommandText" /> against the <see cref="P:System.Data.IDbCommand.Connection" />, and builds an <see cref="T:System.Data.IDataReader" /> using one of the <see cref="T:System.Data.CommandBehavior" /> values.
    /// </summary>
    /// <param name="behavior">One of the <see cref="T:System.Data.CommandBehavior" /> values.</param>
    /// <returns>An <see cref="T:System.Data.IDataReader" /> object.</returns>
    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
        return dbCmd.ExecuteReader(behavior);
    }

    /// <summary>
    /// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
    /// </summary>
    /// <returns>The first column of the first row in the resultset.</returns>
    public object ExecuteScalar()
    {
        return dbCmd.ExecuteScalar();
    }

    /// <summary>
    /// Gets or sets the <see cref="T:System.Data.IDbConnection" /> used by this instance of the <see cref="T:System.Data.IDbCommand" />.
    /// </summary>
    /// <value>The connection.</value>
    public IDbConnection Connection
    {
        get => dbCmd.Connection;
        set => dbCmd.Connection = value;
    }
    /// <summary>
    /// Gets or sets the transaction within which the <see langword="Command" /> object of a .NET Framework data provider executes.
    /// </summary>
    /// <value>The transaction.</value>
    public IDbTransaction Transaction
    {
        get => dbCmd.Transaction;
        set => dbCmd.Transaction = value;
    }
    /// <summary>
    /// Gets or sets the text command to run against the data source.
    /// </summary>
    /// <value>The command text.</value>
    public string CommandText
    {
        get => dbCmd.CommandText;
        set => dbCmd.CommandText = value;
    }
    /// <summary>
    /// Gets or sets the wait time before terminating the attempt to execute a command and generating an error.
    /// </summary>
    /// <value>The command timeout.</value>
    public int CommandTimeout
    {
        get => dbCmd.CommandTimeout;
        set => dbCmd.CommandTimeout = value;
    }
    /// <summary>
    /// Indicates or specifies how the <see cref="P:System.Data.IDbCommand.CommandText" /> property is interpreted.
    /// </summary>
    /// <value>The type of the command.</value>
    public CommandType CommandType
    {
        get => dbCmd.CommandType;
        set => dbCmd.CommandType = value;
    }
    /// <summary>
    /// Gets the <see cref="T:System.Data.IDataParameterCollection" />.
    /// </summary>
    /// <value>The parameters.</value>
    public IDataParameterCollection Parameters => dbCmd.Parameters;

    /// <summary>
    /// Gets or sets how command results are applied to the <see cref="T:System.Data.DataRow" /> when used by the <see cref="M:System.Data.IDataAdapter.Update(System.Data.DataSet)" /> method of a <see cref="T:System.Data.Common.DbDataAdapter" />.
    /// </summary>
    /// <value>The updated row source.</value>
    public UpdateRowSource UpdatedRowSource
    {
        get => dbCmd.UpdatedRowSource;
        set => dbCmd.UpdatedRowSource = value;
    }

    /// <summary>
    /// Gets the database command.
    /// </summary>
    /// <value>The database command.</value>
    public IDbCommand DbCommand => dbCmd;
}