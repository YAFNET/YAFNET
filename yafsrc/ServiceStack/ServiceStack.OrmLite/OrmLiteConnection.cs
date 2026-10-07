// ***********************************************************************
// <copyright file="OrmLiteConnection.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.Data;
using ServiceStack.Model;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

using ServiceStack.Logging;

using System;

/// <summary>
/// Wrapper IDbConnection class to allow for connection sharing, mocking, etc.
/// </summary>
public class OrmLiteConnection
    : IDbConnection, IHasDbConnection, IHasDbTransaction, ISetDbTransaction, IHasDialectProvider, IHasTag, IHasDbWrites
{
    /// <summary>
    /// The factory
    /// </summary>
    public readonly OrmLiteConnectionFactory Factory;

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    /// <value>The tag.</value>
    public string? Tag { get; set; }

    /// <summary>
    /// Gets or sets the transaction.
    /// </summary>
    /// <value>The transaction.</value>
    public IDbTransaction? Transaction
    {
        get => this.source != null ? this.source.Transaction : this.transaction;
        set
        {
            if (this.source != null)
            {
                this.source.Transaction = value;
            }
            else
            {
                this.transaction = value;
            }
        }
    }

    /// <summary>
    /// The transaction
    /// </summary>
    private IDbTransaction? transaction;

    /// <summary>
    /// Gets the database transaction.
    /// </summary>
    /// <value>The database transaction.</value>
    public IDbTransaction? DbTransaction => this.Transaction;

    /// <summary>
    /// The database connection
    /// </summary>
    private IDbConnection? dbConnection;

    /// <summary>
    /// The connection this is an unfiltered view of, when created by WithoutFilters(), which shares its underlying
    /// connection and transaction
    /// </summary>
    private OrmLiteConnection? source;

    /// <summary>
    /// Whether the connection is for queries that can read from a read replica, opened with
    /// OpenReadOnlyDbConnection(). Its statements can't write, even when it's opened on the primary, e.g. in
    /// development without a replica, so writes fail the same way they would on a replica.
    /// </summary>
    public bool IsReadOnly { get; internal set; }

    /// <summary>
    /// Whether the connection is to a read replica
    /// </summary>
    public bool IsReadReplica { get; internal set; }

    /// <summary>
    /// Whether the connection has run a statement that writes, e.g. so a request reads what it wrote from the primary
    /// </summary>
    public bool HasWrites
    {
        get => this.source?.HasWrites ?? this.hasWrites;
        private set
        {
            if (this.source != null)
            {
                this.source.HasWrites = value;
            }
            else
            {
                this.hasWrites = value;
            }
        }
    }
    private bool hasWrites;

    /// <summary>
    /// Called before the connection runs its first statement that writes, e.g. so the user's next requests read what
    /// they wrote from the primary
    /// </summary>
    public Action? OnFirstWrite { get; set; }

    // The database's session is read-only, and is made read-write again before the connection is pooled
    private bool readOnlySession;

    /// <summary>
    /// Called before each statement runs: records writes, and rejects them on read-only connections the database
    /// doesn't make read-only
    /// </summary>
    internal void OnExecute(IDbCommand dbCmd, bool isNonQuery)
    {
        var owner = this.source ?? this;
        if (owner.IsReadOnly && !owner.readOnlySession && OrmLiteWriteStatement.IsWrite(dbCmd, exact: true))
        {
            throw new InvalidOperationException(
                "This connection is read-only, it's opened for queries that can read from a read replica. " +
                "Write with a connection to the primary, e.g. Db instead of ReadDb, or OpenDbConnection().");
        }
        // Read-only connections can't write
        if (!owner.IsReadOnly && !owner.hasWrites && (isNonQuery || OrmLiteWriteStatement.IsWrite(dbCmd, exact: false)))
        {
            owner.hasWrites = true;
            (this.OnFirstWrite ?? owner.OnFirstWrite)?.Invoke();
        }
    }

    // Make the session of a read-only connection read-only, so the database rejects its writes, including on a
    // replica that isn't read-only, e.g. in development
    private void StartReadOnlySession()
    {
        if (!this.IsReadOnly || this.readOnlySession)
        {
            return;
        }

        var sql = this.DialectProvider.ToReadOnlySessionStatement(true);
        if (sql == null)
        {
            return; // checked by OnExecute()
        }

        using var cmd = this.DbConnection.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
        this.readOnlySession = true;
    }

    // Pooled connections keep their session, so it's made read-write again before the connection is closed
    private void EndReadOnlySession()
    {
        if (!this.readOnlySession)
        {
            return;
        }

        this.readOnlySession = false;
        if (this.dbConnection is not { State: ConnectionState.Open })
        {
            return;
        }

        try
        {
            using var cmd = this.dbConnection.CreateCommand();
            cmd.CommandText = this.DialectProvider.ToReadOnlySessionStatement(false);
            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            // A read replica can't be made read-write, e.g. a PostgreSQL standby, whose pool is only used for reads
            var log = LogManager.GetLogger(this.GetType());
            if (this.IsReadReplica)
            {
                log.Debug("A read replica's connection can't be made read-write before closing it: " + e.Message);
            }
            else
            {
                log.Error("Failed to make a read-only connection read-write before closing it", e);
            }
        }
    }

    /// <summary>
    /// Gets the dialect provider.
    /// </summary>
    /// <value>The dialect provider.</value>
    public IOrmLiteDialectProvider DialectProvider { get; set; }

    /// <summary>
    /// Gets or sets the last command text.
    /// </summary>
    /// <value>The last command text.</value>
    public string? LastCommandText { get; set; }

    /// <summary>
    /// Gets or sets the last command.
    /// </summary>
    /// <value>The last command.</value>
    public IDbCommand? LastCommand { get; set; }

    public string? NamedConnection { get; set; }

    /// <summary>
    /// Gets or sets the wait time before terminating the attempt to execute a command and generating an error(in seconds).
    /// </summary>
    public int? CommandTimeout { get; set; }

    /// <summary>
    /// Gets or sets the connection identifier.
    /// </summary>
    /// <value>The connection identifier.</value>
    public Guid ConnectionId { get; set; }
    public object? WriteLock { get; set; }

    /// <summary>
    /// Mandatory filters and write rules applied to statements on this connection, see db.UseFilters()
    /// </summary>
    /// <value>The filters.</value>
    public OrmLiteConnectionFilters Filters { get; internal set; } = OrmLiteConnectionFilters.Empty;

    private System.Collections.Generic.Dictionary<string, object?>? items;

    /// <summary>
    /// State to keep with this connection for as long as it's open, e.g. the tenant it's confined to, see db.SetItem().
    /// Shared with the connections returned by WithoutFilters(), which are the same connection.
    /// </summary>
    public System.Collections.Generic.Dictionary<string, object?> Items => this.source != null
        ? this.source.Items
        : this.items ??= new();

    /// <summary>
    /// Whether this connection was returned by WithoutFilters()
    /// </summary>
    public bool IsWithoutFilters => this.source != null;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrmLiteConnection" /> class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    public OrmLiteConnection(OrmLiteConnectionFactory factory)
    {
        this.Factory = factory;
        this.DialectProvider = factory.DialectProvider;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrmLiteConnection"/> class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="transaction">The transaction.</param>
    public OrmLiteConnection(OrmLiteConnectionFactory factory, IDbConnection connection, IDbTransaction? transaction = null)
        : this(factory)
    {
        this.dbConnection = connection;
        if (transaction != null)
        {
            this.Transaction = transaction;
        }
    }

    /// <summary>
    /// Gets the database connection.
    /// </summary>
    /// <value>The database connection.</value>
    public IDbConnection DbConnection => this.source != null
        ? this.source.DbConnection
        : this.dbConnection ??= this.ConnectionString.ToDbConnection(this.Factory.DialectProvider);

    /// <summary>
    /// A connection over the same underlying connection and transaction, without this connection's filters and
    /// rules. Disposing or closing it doesn't close the underlying connection.
    /// </summary>
    /// <returns>OrmLiteConnection.</returns>
    internal OrmLiteConnection CreateWithoutFilters()
    {
        var owner = this.source ?? this;
        return new OrmLiteConnection(this.Factory)
        {
            source = owner,
            DialectProvider = owner.DialectProvider,
            Tag = owner.Tag,
            NamedConnection = owner.NamedConnection,
            IsReadOnly = owner.IsReadOnly,
            IsReadReplica = owner.IsReadReplica,
            CommandTimeout = owner.CommandTimeout,
            ConnectionId = owner.ConnectionId,
            WriteLock = owner.WriteLock,
            ConnectionString = owner.ConnectionString
        };
    }

    /// <summary>
    /// The number of times a shared connection, e.g. SQLite :memory:, is open
    /// </summary>
    private int sharedOpens;

    /// <summary>
    /// Open a shared connection which is returned for every open, e.g. SQLite :memory:
    /// </summary>
    /// <returns>OrmLiteConnection.</returns>
    internal OrmLiteConnection OpenShared()
    {
        Interlocked.Increment(ref sharedOpens);
        return this;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        if (this.source != null)
        {
            return; // the underlying connection is owned by the connection it was created from
        }

        this.Factory.OnDispose?.Invoke(this);
        if (!this.Factory.AutoDisposeConnection)
        {
            // Filters and items of a shared connection are scoped to its outermost open so they aren't used by the next open
            if (Interlocked.Decrement(ref this.sharedOpens) <= 0)
            {
                Interlocked.Exchange(ref this.sharedOpens, 0);
                this.Filters = OrmLiteConnectionFilters.Empty;
                this.items = null;
            }
            return;
        }

        if (this.dbConnection == null)
        {
            LogManager.GetLogger(this.GetType()).WarnFormat("No dbConnection to Dispose()");
            return;
        }

        try
        {
            this.EndReadOnlySession();
            this.DialectProvider.OnDisposeConnection?.Invoke(this);
            this.dbConnection?.Dispose();
        }
        catch (Exception e)
        {
            LogManager.GetLogger(this.GetType()).Error("Failed to Dispose()", e);
        }

        this.dbConnection = null;
    }

    /// <summary>
    /// Begins a database transaction.
    /// </summary>
    /// <returns>An object representing the new transaction.</returns>
    public IDbTransaction BeginTransaction()
    {
        return this.Factory.AlwaysReturnTransaction ?? this.DbConnection.BeginTransaction();
    }

    /// <summary>
    /// Begins the transaction.
    /// </summary>
    /// <param name="isolationLevel">The isolation level.</param>
    /// <returns>IDbTransaction.</returns>
    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
    {
        return this.Factory.AlwaysReturnTransaction ?? this.DbConnection.BeginTransaction(isolationLevel);
    }

    /// <summary>
    /// Closes the connection to the database.
    /// </summary>
    public void Close()
    {
        if (this.source != null)
        {
            return;
        }

        if (this.dbConnection == null)
        {
            LogManager.GetLogger(this.GetType()).WarnFormat("No dbConnection to Close()");
            return;
        }

        var id = Diagnostics.OrmLite.WriteConnectionCloseBefore(this);
        var connectionId = this.dbConnection.GetConnectionId();

        Exception? e = null;
        try
        {
            this.EndReadOnlySession();
            this.DialectProvider.OnDisposeConnection?.Invoke(this);
            this.dbConnection.Close();
        }
        catch (Exception ex)
        {
            e = ex;
            throw;
        }
        finally
        {
            if (e != null)
            {
                Diagnostics.OrmLite.WriteConnectionCloseError(id, connectionId, this, e);
            }
            else
            {
                Diagnostics.OrmLite.WriteConnectionCloseAfter(id, connectionId, this);
            }
        }
    }

    /// <summary>
    /// Changes the current database for an open <see langword="Connection" /> object.
    /// </summary>
    /// <param name="databaseName">The name of the database to use in place of the current database.</param>
    public void ChangeDatabase(string databaseName)
    {
        this.DbConnection.ChangeDatabase(databaseName);
    }

    /// <summary>
    /// Creates and returns a Command object associated with the connection.
    /// </summary>
    /// <returns>A Command object associated with the connection.</returns>
    public IDbCommand CreateCommand()
    {
        if (this.Factory.AlwaysReturnCommand != null)
        {
            return this.Factory.AlwaysReturnCommand;
        }

        var cmd = this.DbConnection.CreateCommand();

        return cmd;
    }

    /// <summary>
    /// Opens a database connection with the settings specified by the <see langword="ConnectionString" /> property of the provider-specific Connection object.
    /// </summary>
    public void Open()
    {
        if (this.source != null)
        {
            this.source.Open();
            return;
        }

        var dbConn = this.DbConnection;
        if (dbConn.State == ConnectionState.Broken)
        {
            dbConn.Close();
        }

        if (dbConn.State != ConnectionState.Closed)
        {
            return;
        }

        for (var retry = 1; ; retry++)
        {
            var id = Diagnostics.OrmLite.WriteConnectionOpenBefore(this);
            Exception? e = null;
            TimeSpan delay;

            try
            {
                dbConn.Open();
                //so the internal connection is wrapped for example by miniprofiler
                this.dbConnection = this.Factory.ConnectionFilter(dbConn);

                this.DialectProvider.InitConnection(this);
                this.StartReadOnlySession();
                return;
            }
            catch (Exception ex)
            {
                e = ex;
                if (!(this.DialectProvider.RetryPolicy is { } policy
                      && OrmLiteRetryExec.ShouldRetryOpen(policy, this.DialectProvider, ex, retry, out delay)))
                {
                    throw;
                }
            }
            finally
            {
                if (e != null)
                {
                    Diagnostics.OrmLite.WriteConnectionOpenError(id, this, e);
                }
                else
                {
                    Diagnostics.OrmLite.WriteConnectionOpenAfter(id, this);
                }
            }

            if (dbConn.State != ConnectionState.Closed)
            {
                dbConn.Close();
            }

            Thread.Sleep(delay);
        }
    }

    /// <summary>
    /// Open as an asynchronous operation.
    /// </summary>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task OpenAsync(CancellationToken token = default)
    {
        if (this.source != null)
        {
            await this.source.OpenAsync(token).ConfigAwait();
            return;
        }

        var dbConn = this.DbConnection;
        if (dbConn.State == ConnectionState.Broken)
        {
            dbConn.Close();
        }

        if (dbConn.State != ConnectionState.Closed)
        {
            return;
        }

        for (var retry = 1; ; retry++)
        {
            var id = Diagnostics.OrmLite.WriteConnectionOpenBefore(this);
            Exception? e = null;
            TimeSpan delay;

            try
            {
                await this.DialectProvider.OpenAsync(dbConn, token).ConfigAwait();
                //so the internal connection is wrapped for example by miniprofiler
                this.dbConnection = this.Factory.ConnectionFilter(dbConn);

                this.DialectProvider.InitConnection(this);
                this.StartReadOnlySession();
                return;
            }
            catch (Exception ex)
            {
                e = ex;
                if (!(this.DialectProvider.RetryPolicy is { } policy
                      && OrmLiteRetryExec.ShouldRetryOpen(policy, this.DialectProvider, ex, retry, out delay)))
                {
                    throw;
                }
            }
            finally
            {
                if (e != null)
                {
                    Diagnostics.OrmLite.WriteConnectionOpenError(id, this, e);
                }
                else
                {
                    Diagnostics.OrmLite.WriteConnectionOpenAfter(id, this);
                }
            }

            if (dbConn.State != ConnectionState.Closed)
            {
                dbConn.Close();
            }

            await Task.Delay(delay, token).ConfigAwait();
        }
    }

    /// <summary>
    /// Gets or sets the string used to open a database.
    /// </summary>
    /// <value>The connection string.</value>
    public string? ConnectionString {
        get => field ?? this.Factory.ConnectionString;
        set;
    }

    /// <summary>
    /// Gets the time to wait while trying to establish a connection before terminating the attempt and generating an error.
    /// </summary>
    /// <value>The connection timeout.</value>
    public int ConnectionTimeout => this.DbConnection.ConnectionTimeout;

    /// <summary>
    /// Gets the name of the current database or the database to be used after a connection is opened.
    /// </summary>
    /// <value>The database.</value>
    public string Database => this.DbConnection.Database;

    /// <summary>
    /// Gets the current state of the connection.
    /// </summary>
    /// <value>The state.</value>
    public ConnectionState State => this.DbConnection.State;

    /// <summary>
    /// Gets or sets a value indicating whether [automatic dispose connection].
    /// </summary>
    /// <value><c>true</c> if [automatic dispose connection]; otherwise, <c>false</c>.</value>
    public bool AutoDisposeConnection { get; set; }

    /// <summary>
    /// Performs an explicit conversion from <see cref="OrmLiteConnection" /> to <see cref="DbConnection" />.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator DbConnection(OrmLiteConnection dbConn)
    {
        return (DbConnection)dbConn.DbConnection;
    }
}

/// <summary>
/// Interface ISetDbTransaction
/// </summary>
internal interface ISetDbTransaction
{
    /// <summary>
    /// Gets or sets the transaction.
    /// </summary>
    /// <value>The transaction.</value>
    IDbTransaction Transaction { get; set; }
}

/// <summary>
/// Class OrmLiteConnectionUtils.
/// </summary>
public static class OrmLiteConnectionUtils
{
    /// <param name="db">The database.</param>
    extension(IDbConnection db)
    {
        /// <summary>
        /// Ins the transaction.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool InTransaction()
        {
            return db is IHasDbTransaction { DbTransaction: { } };
        }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <returns>IDbTransaction.</returns>
        public IDbTransaction? GetTransaction()
        {
            return db is IHasDbTransaction setDb ? setDb.DbTransaction : null;
        }
    }
}