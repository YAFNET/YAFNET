// ***********************************************************************
// <copyright file="ProfiledConnection.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Data;
using System.Data.Common;

using ServiceStack.Data;

namespace ServiceStack.MiniProfiler.Data;

/// <summary>
/// Wraps a database connection, allowing sql execution timings to be collected when a <see cref="IDbProfiler" /> session is started.
/// </summary>
public class ProfiledConnection : DbConnection, IHasDbConnection
{
    /// <summary>
    /// Returns a new <see cref="ProfiledConnection" /> that wraps <paramref name="connection" />,
    /// providing query execution profiling.  If profiler is null, no profiling will occur.
    /// </summary>
    /// <param name="connection">Your provider-specific flavor of connection, e.g. SqlConnection, OracleConnection</param>
    /// <param name="profiler">The currently started <see cref="IDbProfiler" /> or null.</param>
    /// <param name="autoDisposeConnection">Determines whether the ProfiledDbConnection will dispose the underlying connection.</param>
    public ProfiledConnection(DbConnection connection, IDbProfiler profiler, bool autoDisposeConnection = true)
    {
        this.Init(connection, profiler, autoDisposeConnection);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfiledConnection" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="profiler">The profiler.</param>
    /// <param name="autoDisposeConnection">if set to <c>true</c> [automatic dispose connection].</param>
    /// <exception cref="System.ArgumentException"></exception>
    public ProfiledConnection(IDbConnection connection, IDbProfiler profiler, bool autoDisposeConnection = true)
    {
        if (connection is IHasDbConnection hasConn)
        {
            connection = hasConn.DbConnection;
        }

        if (connection is not DbConnection dbConn)
        {
            throw new ArgumentException(connection.GetType().FullName + " does not inherit DbConnection");
        }

        this.Init(dbConn, profiler, autoDisposeConnection);
    }

    /// <summary>
    /// Initializes the specified connection.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="profiler">The profiler.</param>
    /// <param name="autoDisposeConnection">if set to <c>true</c> [automatic dispose connection].</param>
    /// <exception cref="System.ArgumentNullException">connection</exception>
    private void Init(DbConnection connection, IDbProfiler profiler, bool autoDisposeConnection)
    {
        this.AutoDisposeConnection = autoDisposeConnection;
        this.InnerConnection = connection ?? throw new ArgumentNullException(nameof(connection));
        this.InnerConnection.StateChange += this.StateChangeHandler;

        if (profiler != null)
        {
            this.Profiler = profiler;
        }
    }

    /// <summary>
    /// The underlying, real database connection to your db provider.
    /// </summary>
    /// <value>The inner connection.</value>
    public DbConnection InnerConnection { get; protected set; }

    /// <summary>
    /// Gets the database connection.
    /// </summary>
    /// <value>The database connection.</value>
    public IDbConnection DbConnection => this.InnerConnection;

    /// <summary>
    /// The current profiler instance; could be null.
    /// </summary>
    /// <value>The profiler.</value>
    public IDbProfiler Profiler { get; protected set; }

    /// <summary>
    /// The raw connection this is wrapping
    /// </summary>
    /// <value>The wrapped connection.</value>
    public DbConnection WrappedConnection => this.InnerConnection;

    /// <summary>
    /// Gets a value indicating whether the component can raise an event.
    /// </summary>
    /// <value><c>true</c> if this instance can raise events; otherwise, <c>false</c>.</value>
    override protected bool CanRaiseEvents => true;

    /// <summary>
    /// Gets or sets the string used to open the connection.
    /// </summary>
    /// <value>The connection string.</value>
    public override string ConnectionString
    {
        get => this.InnerConnection.ConnectionString;
        set => this.InnerConnection.ConnectionString = value;
    }

    /// <summary>
    /// Gets the time to wait while establishing a connection before terminating the attempt and generating an error.
    /// </summary>
    /// <value>The connection timeout.</value>
    public override int ConnectionTimeout => this.InnerConnection.ConnectionTimeout;

    /// <summary>
    /// Gets the name of the current database after a connection is opened, or the database name specified in the connection string before the connection is opened.
    /// </summary>
    /// <value>The database.</value>
    public override string Database => this.InnerConnection.Database;

    /// <summary>
    /// Gets the name of the database server to which to connect.
    /// </summary>
    /// <value>The data source.</value>
    public override string DataSource => this.InnerConnection.DataSource;

    /// <summary>
    /// Gets a string that represents the version of the server to which the object is connected.
    /// </summary>
    /// <value>The server version.</value>
    public override string ServerVersion => this.InnerConnection.ServerVersion;

    /// <summary>
    /// Gets a string that describes the state of the connection.
    /// </summary>
    /// <value>The state.</value>
    public override ConnectionState State => this.InnerConnection.State;

    /// <summary>
    /// Gets or sets a value indicating whether [automatic dispose connection].
    /// </summary>
    /// <value><c>true</c> if [automatic dispose connection]; otherwise, <c>false</c>.</value>
    protected bool AutoDisposeConnection { get; set; }

    /// <summary>
    /// Changes the current database for an open connection.
    /// </summary>
    /// <param name="databaseName">Specifies the name of the database for the connection to use.</param>
    public override void ChangeDatabase(string databaseName)
    {
        this.InnerConnection.ChangeDatabase(databaseName);
    }

    /// <summary>
    /// Closes the connection to the database. This is the preferred method of closing any open connection.
    /// </summary>
    public override void Close()
    {
        if (this.AutoDisposeConnection)
        {
            this.InnerConnection.Close();
        }
    }

    /// <summary>
    /// Opens a database connection with the settings specified by the <see cref="P:System.Data.Common.DbConnection.ConnectionString" />.
    /// </summary>
    public override void Open()
    {
        if (this.InnerConnection.State != ConnectionState.Open)
        {
            this.InnerConnection.Open();
        }
    }

    /// <summary>
    /// Starts a database transaction.
    /// </summary>
    /// <param name="isolationLevel">Specifies the isolation level for the transaction.</param>
    /// <returns>An object representing the new transaction.</returns>
    override protected DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    {
        return new ProfiledDbTransaction(this.InnerConnection.BeginTransaction(isolationLevel), this);
    }

    /// <summary>
    /// Creates and returns a <see cref="T:System.Data.Common.DbCommand" /> object associated with the current connection.
    /// </summary>
    /// <returns>A <see cref="T:System.Data.Common.DbCommand" /> object.</returns>
    override protected DbCommand CreateDbCommand()
    {
        return new ProfiledCommand(this.InnerConnection.CreateCommand(), this, this.Profiler);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component" /> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
    override protected void Dispose(bool disposing)
    {
        if (disposing && this.InnerConnection != null)
        {
            this.InnerConnection.StateChange -= this.StateChangeHandler;
            if (this.AutoDisposeConnection)
            {
                this.InnerConnection.Dispose();
            }
        }

        this.InnerConnection = null;
        this.Profiler = null;
        base.Dispose(disposing);
    }

    /// <summary>
    /// States the change handler.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="StateChangeEventArgs" /> instance containing the event data.</param>
    private void StateChangeHandler(object sender, StateChangeEventArgs e)
    {
        this.OnStateChange(e);
    }
}