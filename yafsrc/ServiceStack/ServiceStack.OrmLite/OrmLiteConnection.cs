// ***********************************************************************
// <copyright file="OrmLiteConnection.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Data;
using ServiceStack.Text;

namespace ServiceStack.OrmLite;

using ServiceStack.Logging;
using System;
using System.Drawing;

/// <summary>
/// Wrapper IDbConnection class to allow for connection sharing, mocking, etc.
/// </summary>
public class OrmLiteConnection
    : IDbConnection, IHasDbConnection, IHasDbTransaction, ISetDbTransaction, IHasDialectProvider
{
    /// <summary>
    /// The factory
    /// </summary>
    public readonly OrmLiteConnectionFactory Factory;
    /// <summary>
    /// Gets or sets the transaction.
    /// </summary>
    /// <value>The transaction.</value>
    public IDbTransaction Transaction { get; set; }
    /// <summary>
    /// Gets the database transaction.
    /// </summary>
    /// <value>The database transaction.</value>
    public IDbTransaction DbTransaction => Transaction;
    /// <summary>
    /// The database connection
    /// </summary>
    private IDbConnection dbConnection;

    /// <summary>
    /// Gets the dialect provider.
    /// </summary>
    /// <value>The dialect provider.</value>
    public IOrmLiteDialectProvider DialectProvider { get; set; }
    /// <summary>
    /// Gets or sets the last command text.
    /// </summary>
    /// <value>The last command text.</value>
    public string LastCommandText { get; set; }
    /// <summary>
    /// Gets or sets the command timeout.
    /// </summary>
    /// <value>The command timeout.</value>
    public int? CommandTimeout { get; set; }
    public Guid ConnectionId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrmLiteConnection"/> class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    public OrmLiteConnection(OrmLiteConnectionFactory factory)
    {
        this.Factory = factory;
        this.DialectProvider = factory.DialectProvider;
    }

    /// <summary>
    /// Gets the database connection.
    /// </summary>
    /// <value>The database connection.</value>
    public IDbConnection DbConnection => dbConnection ??= ConnectionString.ToDbConnection(Factory.DialectProvider);

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Factory.OnDispose?.Invoke(this);
        if (!Factory.AutoDisposeConnection) return;

        if (dbConnection == null)
        {
            LogManager.GetLogger(GetType()).WarnFormat("No dbConnection to Dispose()");
            return;
        }

        dbConnection?.Dispose();
        dbConnection = null;
    }

    /// <summary>
    /// Begins a database transaction.
    /// </summary>
    /// <returns>An object representing the new transaction.</returns>
    public IDbTransaction BeginTransaction()
    {
        return Factory.AlwaysReturnTransaction ?? DbConnection.BeginTransaction();
    }

    /// <summary>
    /// Begins the transaction.
    /// </summary>
    /// <param name="isolationLevel">The isolation level.</param>
    /// <returns>IDbTransaction.</returns>
    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
    {
        return Factory.AlwaysReturnTransaction ?? DbConnection.BeginTransaction(isolationLevel);
    }

    /// <summary>
    /// Closes the connection to the database.
    /// </summary>
    public void Close()
    {
        if (dbConnection == null)
        {
            LogManager.GetLogger(GetType()).WarnFormat("No dbConnection to Close()");
            return;
        }

        var id = Diagnostics.OrmLite.WriteConnectionCloseBefore(dbConnection);
        var connectionId = dbConnection.GetConnectionId();
        Exception e = null;
        try
        {
            dbConnection.Close();
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
                Diagnostics.OrmLite.WriteConnectionCloseError(id, connectionId, dbConnection, e);
            }
            else
            {
                Diagnostics.OrmLite.WriteConnectionCloseAfter(id, connectionId, dbConnection);
            }
        }
    }

    /// <summary>
    /// Changes the current database for an open <see langword="Connection" /> object.
    /// </summary>
    /// <param name="databaseName">The name of the database to use in place of the current database.</param>
    public void ChangeDatabase(string databaseName)
    {
        DbConnection.ChangeDatabase(databaseName);
    }

    /// <summary>
    /// Creates and returns a Command object associated with the connection.
    /// </summary>
    /// <returns>A Command object associated with the connection.</returns>
    public IDbCommand CreateCommand()
    {
        if (Factory.AlwaysReturnCommand != null)
            return Factory.AlwaysReturnCommand;

        var cmd = DbConnection.CreateCommand();

        return cmd;
    }

    /// <summary>
    /// Opens a database connection with the settings specified by the <see langword="ConnectionString" /> property of the provider-specific Connection object.
    /// </summary>
    public void Open()
    {
        var dbConn = DbConnection;
        if (dbConn.State == ConnectionState.Broken)
            dbConn.Close();

        if (dbConn.State != ConnectionState.Closed)
        {
            return;
        }

        var id = Diagnostics.OrmLite.WriteConnectionOpenBefore(dbConn);
        Exception e = null;

        try
        {
            dbConn.Open();
            //so the internal connection is wrapped for example by miniprofiler
            if (Factory.ConnectionFilter != null)
                dbConn = Factory.ConnectionFilter(dbConn);

            DialectProvider.InitConnection(dbConn);
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
                Diagnostics.OrmLite.WriteConnectionOpenError(id, dbConn, e);
            }
            else
            {
                Diagnostics.OrmLite.WriteConnectionOpenAfter(id, dbConn);
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
        var dbConn = DbConnection;
        if (dbConn.State == ConnectionState.Broken)
            dbConn.Close();

        if (dbConn.State == ConnectionState.Closed)
        {
            var id = Diagnostics.OrmLite.WriteConnectionOpenBefore(dbConn);
            Exception e = null;

            try
            {
                await DialectProvider.OpenAsync(dbConn, token).ConfigAwait();
                //so the internal connection is wrapped for example by miniprofiler
                if (Factory.ConnectionFilter != null)
                    dbConn = Factory.ConnectionFilter(dbConn);

                DialectProvider.InitConnection(dbConn);
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
                    Diagnostics.OrmLite.WriteConnectionOpenError(id, dbConn, e);
                }
                else
                {
                    Diagnostics.OrmLite.WriteConnectionOpenAfter(id, dbConn);
                }
            }
        }
    }

    /// <summary>
    /// The connection string
    /// </summary>
    private string connectionString;
    /// <summary>
    /// Gets or sets the string used to open a database.
    /// </summary>
    /// <value>The connection string.</value>
    public string ConnectionString
    {
        get => connectionString ?? Factory.ConnectionString;
        set => connectionString = value;
    }

    /// <summary>
    /// Gets the time to wait while trying to establish a connection before terminating the attempt and generating an error.
    /// </summary>
    /// <value>The connection timeout.</value>
    public int ConnectionTimeout => DbConnection.ConnectionTimeout;

    /// <summary>
    /// Gets the name of the current database or the database to be used after a connection is opened.
    /// </summary>
    /// <value>The database.</value>
    public string Database => DbConnection.Database;

    /// <summary>
    /// Gets the current state of the connection.
    /// </summary>
    /// <value>The state.</value>
    public ConnectionState State => DbConnection.State;

    /// <summary>
    /// Gets or sets a value indicating whether [automatic dispose connection].
    /// </summary>
    /// <value><c>true</c> if [automatic dispose connection]; otherwise, <c>false</c>.</value>
    public bool AutoDisposeConnection { get; set; }

    /// <summary>
    /// Performs an explicit conversion from <see cref="OrmLiteConnection"/> to <see cref="DbConnection"/>.
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
    /// <summary>
    /// Ins the transaction.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool InTransaction(this IDbConnection db) =>
        db is IHasDbTransaction { DbTransaction: { } };

    /// <summary>
    /// Gets the transaction.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <returns>IDbTransaction.</returns>
    public static IDbTransaction GetTransaction(this IDbConnection db) =>
        db is IHasDbTransaction setDb ? setDb.DbTransaction : null;
}