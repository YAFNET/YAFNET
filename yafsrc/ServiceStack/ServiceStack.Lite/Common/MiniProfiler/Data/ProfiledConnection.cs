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
    /// The connection
    /// </summary>
    private DbConnection _conn;
    /// <summary>
    /// The profiler
    /// </summary>
    private IDbProfiler _profiler;

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
    /// Initializes the specified connection.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="profiler">The profiler.</param>
    /// <param name="autoDisposeConnection">if set to <c>true</c> [automatic dispose connection].</param>
    /// <exception cref="System.ArgumentNullException">connection</exception>
    private void Init(DbConnection connection, IDbProfiler profiler, bool autoDisposeConnection)
    {
        this.AutoDisposeConnection = autoDisposeConnection;
        this._conn = connection ?? throw new ArgumentNullException(nameof(connection));
        this._conn.StateChange += this.StateChangeHandler;

        if (profiler != null)
        {
            this._profiler = profiler;
        }
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


#pragma warning disable 1591 // xml doc comments warnings

    /// <summary>
    /// The underlying, real database connection to your db provider.
    /// </summary>
    /// <value>The inner connection.</value>
    public DbConnection InnerConnection
    {
        get => this._conn;
        protected set => this._conn = value;
    }

    /// <summary>
    /// Gets the database connection.
    /// </summary>
    /// <value>The database connection.</value>
    public IDbConnection DbConnection => this._conn;

    /// <summary>
    /// The current profiler instance; could be null.
    /// </summary>
    /// <value>The profiler.</value>
    public IDbProfiler Profiler
    {
        get => this._profiler;
        protected set => this._profiler = value;
    }

    /// <summary>
    /// The raw connection this is wrapping
    /// </summary>
    /// <value>The wrapped connection.</value>
    public DbConnection WrappedConnection => this._conn;

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
        get => this._conn.ConnectionString;
        set => this._conn.ConnectionString = value;
    }

    /// <summary>
    /// Gets the time to wait while establishing a connection before terminating the attempt and generating an error.
    /// </summary>
    /// <value>The connection timeout.</value>
    public override int ConnectionTimeout => this._conn.ConnectionTimeout;

    /// <summary>
    /// Gets the name of the current database after a connection is opened, or the database name specified in the connection string before the connection is opened.
    /// </summary>
    /// <value>The database.</value>
    public override string Database => this._conn.Database;

    /// <summary>
    /// Gets the name of the database server to which to connect.
    /// </summary>
    /// <value>The data source.</value>
    public override string DataSource => this._conn.DataSource;

    /// <summary>
    /// Gets a string that represents the version of the server to which the object is connected.
    /// </summary>
    /// <value>The server version.</value>
    public override string ServerVersion => this._conn.ServerVersion;

    /// <summary>
    /// Gets a string that describes the state of the connection.
    /// </summary>
    /// <value>The state.</value>
    public override ConnectionState State => this._conn.State;

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
        this._conn.ChangeDatabase(databaseName);
    }

    /// <summary>
    /// Closes the connection to the database. This is the preferred method of closing any open connection.
    /// </summary>
    public override void Close()
    {
        if (this.AutoDisposeConnection)
        {
            this._conn.Close();
        }
    }

    //public override void EnlistTransaction(System.Transactions.Transaction transaction)
    //{
    //    _conn.EnlistTransaction(transaction);
    //}
#if !NET7_0_OR_GREATER
    /// <summary>
    /// Returns schema information for the data source of this <see cref="T:System.Data.Common.DbConnection" />.
    /// </summary>
    /// <returns>A <see cref="T:System.Data.DataTable" /> that contains schema information.</returns>
    public override DataTable GetSchema()
    {
        return _conn.GetSchema();
    }

    /// <summary>
    /// Returns schema information for the data source of this <see cref="T:System.Data.Common.DbConnection" /> using the specified string for the schema name.
    /// </summary>
    /// <param name="collectionName">Specifies the name of the schema to return.</param>
    /// <returns>A <see cref="T:System.Data.DataTable" /> that contains schema information.</returns>
    public override DataTable GetSchema(string collectionName)
    {
        return _conn.GetSchema(collectionName);
    }

    /// <summary>
    /// Returns schema information for the data source of this <see cref="T:System.Data.Common.DbConnection" /> using the specified string for the schema name and the specified string array for the restriction values.
    /// </summary>
    /// <param name="collectionName">Specifies the name of the schema to return.</param>
    /// <param name="restrictionValues">Specifies a set of restriction values for the requested schema.</param>
    /// <returns>A <see cref="T:System.Data.DataTable" /> that contains schema information.</returns>
    public override DataTable GetSchema(string collectionName, string[] restrictionValues)
    {
        return _conn.GetSchema(collectionName, restrictionValues);
    }
#endif

    /// <summary>
    /// Opens a database connection with the settings specified by the <see cref="P:System.Data.Common.DbConnection.ConnectionString" />.
    /// </summary>
    public override void Open()
    {
        if (this._conn.State != ConnectionState.Open)
        {
            this._conn.Open();
        }
    }

    /// <summary>
    /// Starts a database transaction.
    /// </summary>
    /// <param name="isolationLevel">Specifies the isolation level for the transaction.</param>
    /// <returns>An object representing the new transaction.</returns>
    override protected DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    {
        return new ProfiledDbTransaction(this._conn.BeginTransaction(isolationLevel), this);
    }

    /// <summary>
    /// Creates and returns a <see cref="T:System.Data.Common.DbCommand" /> object associated with the current connection.
    /// </summary>
    /// <returns>A <see cref="T:System.Data.Common.DbCommand" /> object.</returns>
    override protected DbCommand CreateDbCommand()
    {
        return new ProfiledCommand(this._conn.CreateCommand(), this, this._profiler);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component" /> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
    override protected void Dispose(bool disposing)
    {
        if (disposing && this._conn != null)
        {
            this._conn.StateChange -= this.StateChangeHandler;
            if (this.AutoDisposeConnection)
            {
                this._conn.Dispose();
            }
        }
        this._conn = null;
        this._profiler = null;
        base.Dispose(disposing);
    }

    /// <summary>
    /// States the change handler.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="StateChangeEventArgs" /> instance containing the event data.</param>
    void StateChangeHandler(object sender, StateChangeEventArgs e)
    {
        this.OnStateChange(e);
    }
}

#pragma warning restore 1591 // xml doc comments warnings