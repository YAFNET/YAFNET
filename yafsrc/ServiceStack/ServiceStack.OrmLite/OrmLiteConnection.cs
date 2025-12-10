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
    : IDbConnection, IHasDbConnection, IHasDbTransaction, ISetDbTransaction, IHasDialectProvider, IHasTag
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
    public IDbTransaction? Transaction { get; set; }

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
    public IDbConnection DbConnection =>
        this.dbConnection ??= this.ConnectionString.ToDbConnection(this.Factory.DialectProvider);

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Factory.OnDispose?.Invoke(this);
        if (!this.Factory.AutoDisposeConnection)
        {
            return;
        }

        if (this.dbConnection == null)
        {
            LogManager.GetLogger(this.GetType()).WarnFormat("No dbConnection to Dispose()");
            return;
        }

        try
        {
            this.DialectProvider.OnDisposeConnection?.Invoke(this);
            this.dbConnection?.Dispose();
        }
        catch (Exception e)
        {
            LogManager.GetLogger(this.GetType()).Error("Failed to Dispose()", e);
            Console.WriteLine(e);
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
        var dbConn = this.DbConnection;
        if (dbConn.State == ConnectionState.Broken)
        {
            dbConn.Close();
        }

        if (dbConn.State != ConnectionState.Closed)
        {
            return;
        }

        var id = Diagnostics.OrmLite.WriteConnectionOpenBefore(this);
        Exception? e = null;

        try
        {
            dbConn.Open();
            //so the internal connection is wrapped for example by miniprofiler
            this.dbConnection = this.Factory.ConnectionFilter(dbConn);

            this.DialectProvider.InitConnection(this);
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
                Diagnostics.OrmLite.WriteConnectionOpenError(id, this, e);
            }
            else
            {
                Diagnostics.OrmLite.WriteConnectionOpenAfter(id, this);
            }
        }
    }

    /// <summary>
    /// Open as an asynchronous operation.
    /// </summary>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task OpenAsync(CancellationToken token = default)
    {
        var dbConn = this.DbConnection;
        if (dbConn.State == ConnectionState.Broken)
        {
            dbConn.Close();
        }

        if (dbConn.State == ConnectionState.Closed)
        {
            var id = Diagnostics.OrmLite.WriteConnectionOpenBefore(this);
            Exception? e = null;

            try
            {
                await this.DialectProvider.OpenAsync(dbConn, token).ConfigAwait();
                //so the internal connection is wrapped for example by miniprofiler
                this.dbConnection = this.Factory.ConnectionFilter(dbConn);

                this.DialectProvider.InitConnection(this);
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
                    Diagnostics.OrmLite.WriteConnectionOpenError(id, this, e);
                }
                else
                {
                    Diagnostics.OrmLite.WriteConnectionOpenAfter(id, this);
                }
            }
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