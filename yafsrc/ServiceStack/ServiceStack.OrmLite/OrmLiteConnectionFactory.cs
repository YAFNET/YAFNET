// ***********************************************************************
// <copyright file="OrmLiteConnectionFactory.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Data;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Allow for mocking and unit testing by providing non-disposing
/// connection factory with injectable IDbCommand and IDbTransaction proxies
/// </summary>
public class OrmLiteConnectionFactory : IDbConnectionFactoryExtended, IDbReadOnlyConnectionFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrmLiteConnectionFactory" /> class.
    /// </summary>
    public OrmLiteConnectionFactory()
        : this(null, null, true) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrmLiteConnectionFactory" /> class.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    public OrmLiteConnectionFactory(string connectionString)
        : this(connectionString, null, true) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrmLiteConnectionFactory" /> class.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    public OrmLiteConnectionFactory(string connectionString, IOrmLiteDialectProvider dialectProvider)
        : this(connectionString, dialectProvider, true) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrmLiteConnectionFactory" /> class.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <param name="setGlobalDialectProvider">if set to <c>true</c> [set global dialect provider].</param>
    public OrmLiteConnectionFactory(string connectionString, IOrmLiteDialectProvider dialectProvider, bool setGlobalDialectProvider)
    {
        if (connectionString == "DataSource=:memory:")
        {
            connectionString = ":memory:";
        }

        this.ConnectionString = connectionString;
        this.AutoDisposeConnection = connectionString != ":memory:";
        this.DialectProvider = dialectProvider ?? OrmLiteConfig.DialectProvider;

        if (setGlobalDialectProvider && dialectProvider != null)
        {
            OrmLiteConfig.DialectProvider = dialectProvider;
        }

        this.ConnectionFilter = x => x;

        JsConfig.InitStatics();
    }

    /// <summary>
    /// Gets or sets the dialect provider.
    /// </summary>
    /// <value>The dialect provider.</value>
    public IOrmLiteDialectProvider DialectProvider { get; set; }

    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    /// <value>The connection string.</value>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [automatic dispose connection].
    /// </summary>
    /// <value><c>true</c> if [automatic dispose connection]; otherwise, <c>false</c>.</value>
    public bool AutoDisposeConnection { get; set; }

    /// <summary>
    /// Gets or sets the connection filter.
    /// </summary>
    /// <value>The connection filter.</value>
    public Func<IDbConnection, IDbConnection> ConnectionFilter { get; set; }

    /// <summary>
    /// Force the IDbConnection to always return this IDbCommand
    /// </summary>
    /// <value>The always return command.</value>
    public IDbCommand AlwaysReturnCommand { get; set; }

    /// <summary>
    /// Force the IDbConnection to always return this IDbTransaction
    /// </summary>
    /// <value>The always return transaction.</value>
    public IDbTransaction AlwaysReturnTransaction { get; set; }

    /// <summary>
    /// Gets or sets the on dispose.
    /// </summary>
    /// <value>The on dispose.</value>
    public Action<OrmLiteConnection> OnDispose { get; set; }

    /// <summary>
    /// Gets the orm lite connection.
    /// </summary>
    /// <value>The orm lite connection.</value>
    private OrmLiteConnection OrmLiteConnection => field ??= this.DialectProvider != null
        ? this.DialectProvider.CreateOrmLiteConnection(this)
        : new OrmLiteConnection(this);

    /// <summary>
    /// Creates the database connection.
    /// </summary>
    /// <returns>IDbConnection.</returns>
    /// <exception cref="System.ArgumentNullException">ConnectionString - ConnectionString must be set</exception>
    public virtual IDbConnection CreateDbConnection()
    {
        if (this.ConnectionString == null)
        {
            throw new ArgumentNullException("ConnectionString", "ConnectionString must be set");
        }

        var connection = this.AutoDisposeConnection
                             ? this.DialectProvider.CreateOrmLiteConnection(this)
                             : this.OrmLiteConnection.OpenShared();

        return connection;
    }

    /// <summary>
    /// Uses the specified connection.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="trans">The trans.</param>
    /// <returns>IDbConnection.</returns>
    public virtual IDbConnection Use(IDbConnection connection, IDbTransaction trans = null)
    {
        return new OrmLiteConnection(this, connection, trans);
    }

    /// <summary>
    /// Creates the database connection.
    /// </summary>
    /// <param name="namedConnection">The named connection.</param>
    /// <returns>IDbConnection.</returns>
    /// <exception cref="System.ArgumentNullException">namedConnection</exception>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">No factory registered is named " + namedConnection</exception>
    public static IDbConnection CreateDbConnection(string namedConnection)
    {
        ArgumentNullException.ThrowIfNull(namedConnection);

        if (!NamedConnections.TryGetValue(namedConnection, out var factory))
        {
            throw new KeyNotFoundException("No factory registered is named " + namedConnection);
        }

        IDbConnection connection = factory.AutoDisposeConnection
                                       ? new OrmLiteConnection(factory)
                                       : factory.OrmLiteConnection.OpenShared();
        return connection;
    }

    /// <summary>
    /// Creates the database with write lock.
    /// </summary>
    /// <param name="namedConnection">The named connection.</param>
    /// <returns>DbConnection.</returns>
    /// <exception cref="KeyNotFoundException">No factory registered is named " + namedConnection</exception>
    public DbConnection CreateDbWithWriteLock(string namedConnection = null)
    {
        var factory = this;
        if (namedConnection != null && !NamedConnections.TryGetValue(namedConnection, out factory))
        {
            throw new KeyNotFoundException("No factory registered is named " + namedConnection);
        }

        return new SingleWriterDbConnection(factory, Locks.GetDbLock(namedConnection));
    }

    /// <summary>
    /// Opens the database connection.
    /// </summary>
    /// <returns>IDbConnection.</returns>
    public virtual IDbConnection OpenDbConnection()
    {
        return OpenOrDispose(this.CreateDbConnection());
    }

    /// <summary>
    /// Opens the database connection.
    /// </summary>
    /// <param name="configure">The configure.</param>
    /// <returns>IDbConnection.</returns>
    public virtual IDbConnection OpenDbConnection(Action<IDbConnection> configure)
    {
        return OpenOrDispose(this.CreateDbConnection(), configure);
    }

    /// <summary>
    /// Opens the connection, disposing it if configure or Open() throws so it's not leaked
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>IDbConnection.</returns>
    private static IDbConnection OpenOrDispose(IDbConnection connection, Action<IDbConnection> configure = null)
    {
        try
        {
            configure?.Invoke(connection);
            connection.Open();
            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Opens the connection asynchronous, disposing it if configure or Open() throws so it's not leaked
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="dialect">The dialect.</param>
    /// <param name="configure">The configure.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;IDbConnection&gt; representing the asynchronous operation.</returns>
    private async static Task<IDbConnection> OpenOrDisposeAsync(IDbConnection connection, IOrmLiteDialectProvider dialect,
        Action<IDbConnection> configure, CancellationToken token)
    {
        try
        {
            configure?.Invoke(connection);
            if (connection is OrmLiteConnection ormliteConn)
            {
                await ormliteConn.OpenAsync(token).ConfigAwait();
            }
            else
            {
                await dialect.OpenAsync(connection, token).ConfigAwait();
            }

            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Open database connection as an asynchronous operation.
    /// </summary>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;IDbConnection&gt; representing the asynchronous operation.</returns>
    public async virtual Task<IDbConnection> OpenDbConnectionAsync(CancellationToken token = default)
    {
        return await OpenOrDisposeAsync(this.CreateDbConnection(), this.DialectProvider, null, token).ConfigAwait();
    }

    /// <summary>
    /// Open database connection as an asynchronous operation.
    /// </summary>
    /// <param name="namedConnection">The named connection.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;IDbConnection&gt; representing the asynchronous operation.</returns>
    public async virtual Task<IDbConnection> OpenDbConnectionAsync(string namedConnection, CancellationToken token = default)
    {
        return await OpenOrDisposeAsync(CreateDbConnection(namedConnection), GetNamedDialectProvider(namedConnection), null, token).ConfigAwait();
    }

    /// <summary>
    /// Open database connection as an asynchronous operation.
    /// </summary>
    /// <param name="configure">The configure.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;IDbConnection&gt; representing the asynchronous operation.</returns>
    public async virtual Task<IDbConnection> OpenDbConnectionAsync(Action<IDbConnection> configure, CancellationToken token = default)
    {
        return await OpenOrDisposeAsync(this.CreateDbConnection(), this.DialectProvider, configure, token).ConfigAwait();
    }

    /// <summary>
    /// Open database connection as an asynchronous operation.
    /// </summary>
    /// <param name="namedConnection">The named connection.</param>
    /// <param name="configure">The configure.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;IDbConnection&gt; representing the asynchronous operation.</returns>
    public async virtual Task<IDbConnection> OpenDbConnectionAsync(string namedConnection, Action<IDbConnection> configure, CancellationToken token = default)
    {
        return await OpenOrDisposeAsync(CreateDbConnection(namedConnection), GetNamedDialectProvider(namedConnection), configure, token).ConfigAwait();
    }

    /// <summary>
    /// Gets the dialect provider of a named connection.
    /// </summary>
    /// <param name="namedConnection">The named connection.</param>
    /// <returns>IOrmLiteDialectProvider.</returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">No factory registered is named " + namedConnection</exception>
    private static IOrmLiteDialectProvider GetNamedDialectProvider(string namedConnection)
    {
        return NamedConnections.TryGetValue(namedConnection, out var factory)
                   ? factory.DialectProvider
                   : throw new KeyNotFoundException("No factory registered is named " + namedConnection);
    }

    /// <summary>
    /// Opens the database connection string.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>IDbConnection.</returns>
    /// <exception cref="System.ArgumentNullException">connectionString</exception>
    public virtual IDbConnection OpenDbConnectionString(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        var connection = this.DialectProvider.CreateOrmLiteConnection(this);
        connection.ConnectionString = connectionString;

        return OpenOrDispose(connection);
    }

    public virtual IDbConnection OpenDbConnectionString(string connectionString, Action<IDbConnection> configure)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        var connection = this.DialectProvider.CreateOrmLiteConnection(this);
        connection.ConnectionString = connectionString;

        return OpenOrDispose(connection, configure);
    }

    /// <summary>
    /// Open database connection string as an asynchronous operation.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;IDbConnection&gt; representing the asynchronous operation.</returns>
    /// <exception cref="System.ArgumentNullException">connectionString</exception>
    public async virtual Task<IDbConnection> OpenDbConnectionStringAsync(string connectionString, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        var connection = this.DialectProvider.CreateOrmLiteConnection(this);
        connection.ConnectionString = connectionString;

        return await OpenOrDisposeAsync(connection, this.DialectProvider, null, token).ConfigAwait();
    }

    public async virtual Task<IDbConnection> OpenDbConnectionStringAsync(string connectionString, Action<IDbConnection> configure, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        var connection = this.DialectProvider.CreateOrmLiteConnection(this);
        connection.ConnectionString = connectionString;

        return await OpenOrDisposeAsync(connection, this.DialectProvider, configure, token).ConfigAwait();
    }

    /// <summary>
    /// Opens the database connection string.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="providerName">Name of the provider.</param>
    /// <returns>IDbConnection.</returns>
    /// <exception cref="System.ArgumentNullException">connectionString</exception>
    /// <exception cref="System.ArgumentNullException">providerName</exception>
    /// <exception cref="System.ArgumentException"></exception>
    public virtual IDbConnection OpenDbConnectionString(string connectionString, string providerName)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        ArgumentNullException.ThrowIfNull(providerName);

        if (!DialectProviders.TryGetValue(providerName, out var dialectProvider))
        {
            throw new ArgumentException($"{providerName} is not a registered DialectProvider");
        }

        var dbFactory = new OrmLiteConnectionFactory(connectionString, dialectProvider, setGlobalDialectProvider: false);

        return dbFactory.OpenDbConnection();
    }

    public virtual IDbConnection OpenDbConnectionString(string connectionString, string providerName, Action<IDbConnection> configure)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        ArgumentNullException.ThrowIfNull(providerName);

        if (!DialectProviders.TryGetValue(providerName, out var dialectProvider))
        {
            throw new ArgumentException($"{providerName} is not a registered DialectProvider");
        }

        var dbFactory = new OrmLiteConnectionFactory(connectionString, dialectProvider, setGlobalDialectProvider: false);
        return dbFactory.OpenDbConnection(configure);
    }

    public async virtual Task<IDbConnection> OpenDbConnectionStringAsync(string connectionString, string providerName, Action<IDbConnection> configure, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        ArgumentNullException.ThrowIfNull(providerName);

        if (!DialectProviders.TryGetValue(providerName, out var dialectProvider))
        {
            throw new ArgumentException($"{providerName} is not a registered DialectProvider");
        }

        var dbFactory = new OrmLiteConnectionFactory(connectionString, dialectProvider, setGlobalDialectProvider: false);

        return await dbFactory.OpenDbConnectionAsync(token).ConfigAwait();
    }

    /// <summary>
    /// Open database connection string as an asynchronous operation.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="providerName">Name of the provider.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;IDbConnection&gt; representing the asynchronous operation.</returns>
    /// <exception cref="System.ArgumentNullException">connectionString</exception>
    /// <exception cref="System.ArgumentNullException">providerName</exception>
    /// <exception cref="System.ArgumentException"></exception>
    public async virtual Task<IDbConnection> OpenDbConnectionStringAsync(string connectionString, string providerName, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        ArgumentNullException.ThrowIfNull(providerName);

        if (!DialectProviders.TryGetValue(providerName, out var dialectProvider))
        {
            throw new ArgumentException($"{providerName} is not a registered DialectProvider");
        }

        var dbFactory = new OrmLiteConnectionFactory(connectionString, dialectProvider, setGlobalDialectProvider: false);

        return await dbFactory.OpenDbConnectionAsync(token).ConfigAwait();
    }

    /// <summary>
    /// Opens the database connection.
    /// </summary>
    /// <param name="namedConnection">The named connection.</param>
    /// <returns>IDbConnection.</returns>
    public virtual IDbConnection OpenDbConnection(string namedConnection)
    {
        return OpenOrDispose(CreateDbConnection(namedConnection));
    }
    public virtual IDbConnection OpenDbConnection(string namedConnection, Action<IDbConnection> configure)
    {
        return OpenOrDispose(CreateDbConnection(namedConnection), configure);
    }

    /// <summary>
    /// Gets the dialect providers.
    /// </summary>
    /// <value>The dialect providers.</value>
    public static Dictionary<string, IOrmLiteDialectProvider> DialectProviders => field ??= [];

    /// <summary>
    /// Registers the dialect provider.
    /// </summary>
    /// <param name="providerName">Name of the provider.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    public virtual void RegisterDialectProvider(string providerName, IOrmLiteDialectProvider dialectProvider)
    {
        DialectProviders[providerName] = dialectProvider;
    }

    /// <summary>
    /// Gets the named connections.
    /// </summary>
    /// <value>The named connections.</value>
    public static Dictionary<string, OrmLiteConnectionFactory> NamedConnections => field ??= [];

    /// <summary>
    /// Registers the connection.
    /// </summary>
    /// <param name="namedConnection">The named connection.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    public virtual void RegisterConnection(string namedConnection, string connectionString, IOrmLiteDialectProvider dialectProvider)
    {
        this.RegisterConnection(namedConnection, new OrmLiteConnectionFactory(connectionString, dialectProvider, setGlobalDialectProvider: false));
    }

    /// <summary>
    /// Registers the connection.
    /// </summary>
    /// <param name="namedConnection">The named connection.</param>
    /// <param name="connectionFactory">The connection factory.</param>
    public virtual void RegisterConnection(string namedConnection, OrmLiteConnectionFactory connectionFactory)
    {
        NamedConnections[namedConnection] = connectionFactory;
        Locks.AddLock(namedConnection);
    }

    /// <summary>
    /// The read replica of this connection that OpenReadOnlyDbConnection() opens, e.g. a PostgreSQL standby, or null
    /// when it opens this connection
    /// </summary>
    public OrmLiteConnectionFactory ReadReplica { get; set; }

    /// <summary>
    /// Open this connection when its read replica can't be opened, so reads keep working while it's unavailable,
    /// which is logged as a warning
    /// </summary>
    public bool ReadReplicaFallback { get; set; }

    /// <summary>
    /// Register a read replica of this connection, which uses its dialect. OpenReadOnlyDbConnection() opens it, or this
    /// connection when it can't be opened and fallbackToPrimary.
    /// </summary>
    public virtual void RegisterReadReplica(string connectionString, bool fallbackToPrimary = false)
    {
        this.ReadReplica = CreateReadReplica(this, connectionString);
        this.ReadReplicaFallback = fallbackToPrimary;
    }

    /// <summary>
    /// Register a read replica of a named connection, which uses its dialect. OpenReadOnlyDbConnection(namedConnection)
    /// opens it, or the named connection when it can't be opened and fallbackToPrimary.
    /// </summary>
    public virtual void RegisterReadReplica(string namedConnection, string connectionString, bool fallbackToPrimary = false)
    {
        var primary = GetNamedConnection(namedConnection);
        primary.ReadReplica = CreateReadReplica(primary, connectionString);
        primary.ReadReplicaFallback = fallbackToPrimary;
    }

    private static OrmLiteConnectionFactory CreateReadReplica(OrmLiteConnectionFactory primary, string connectionString) =>
        new(connectionString ?? throw new ArgumentNullException(nameof(connectionString)), primary.DialectProvider,
            setGlobalDialectProvider: false)
        {
            ConnectionFilter = primary.ConnectionFilter,
            OnDispose = primary.OnDispose,
        };

    private static OrmLiteConnectionFactory GetNamedConnection(string namedConnection)
    {
        if (namedConnection == null)
        {
            throw new ArgumentNullException(nameof(namedConnection));
        }

        return NamedConnections.TryGetValue(namedConnection, out var factory)
            ? factory
            : throw new KeyNotFoundException("No factory registered is named " + namedConnection);
    }

    // A read-only connection of this primary or replica, with the name of its named connection. Shared connections,
    // e.g. SQLite :memory:, are the same connection for every open, so they aren't made read-only.
    private IDbConnection CreateReadOnlyConnection(string namedConnection, bool isReplica)
    {
        if (this.ConnectionString == null)
        {
            throw new ArgumentNullException("ConnectionString", "ConnectionString must be set");
        }

        if (!this.AutoDisposeConnection)
        {
            return this.OrmLiteConnection.OpenShared();
        }

        var connection = this.DialectProvider.CreateOrmLiteConnection(this, namedConnection);
        connection.IsReadOnly = true;
        connection.IsReadReplica = isReplica;
        return connection;
    }

    private static IDbConnection OpenReadOnly(OrmLiteConnectionFactory primary, string namedConnection,
        Action<IDbConnection> configure)
    {
        if (primary.ReadReplica is { } replica)
        {
            try
            {
                return OpenOrDispose(replica.CreateReadOnlyConnection(namedConnection, isReplica: true), configure);
            }
            catch (Exception e) when (primary.ReadReplicaFallback)
            {
                LogReplicaFallback(namedConnection, e);
            }
        }
        return OpenOrDispose(primary.CreateReadOnlyConnection(namedConnection, isReplica: false), configure);
    }

    private static async Task<IDbConnection> OpenReadOnlyAsync(OrmLiteConnectionFactory primary, string namedConnection,
        Action<IDbConnection> configure, CancellationToken token)
    {
        if (primary.ReadReplica is { } replica)
        {
            try
            {
                return await OpenOrDisposeAsync(replica.CreateReadOnlyConnection(namedConnection, isReplica: true),
                    replica.DialectProvider, configure, token).ConfigAwait();
            }
            catch (Exception e) when (primary.ReadReplicaFallback && !token.IsCancellationRequested)
            {
                LogReplicaFallback(namedConnection, e);
            }
        }
        return await OpenOrDisposeAsync(primary.CreateReadOnlyConnection(namedConnection, isReplica: false),
            primary.DialectProvider, configure, token).ConfigAwait();
    }

    private static void LogReplicaFallback(string namedConnection, Exception e) =>
        OrmLiteLog.Log.Warn((namedConnection != null ? $"The read replica of {namedConnection}" : "The read replica") +
            " can't be opened, reading from its primary instead: " + e.Message, e);

    /// <summary>
    /// Open a read-only connection for queries that can read from a replica: its read replica when one is registered,
    /// otherwise this connection. Statements that write are rejected on both, so writes fail the same way with and
    /// without a replica. A replica can be behind the primary, so read what was just written from the primary.
    /// </summary>
    public virtual IDbConnection OpenReadOnlyDbConnection() => this.OpenReadOnlyDbConnection((Action<IDbConnection>)null);

    /// <summary>
    /// Open a read-only connection for queries that can read from a replica, configured before it's opened
    /// </summary>
    public virtual IDbConnection OpenReadOnlyDbConnection(Action<IDbConnection> configure) =>
        OpenReadOnly(this, null, configure);

    /// <summary>
    /// Open a read-only connection for queries that can read from a replica: the named connection's read replica when
    /// one is registered, otherwise the named connection
    /// </summary>
    public virtual IDbConnection OpenReadOnlyDbConnection(string namedConnection) =>
        this.OpenReadOnlyDbConnection(namedConnection, null);

    /// <summary>
    /// Open a read-only connection of a named connection for queries that can read from a replica, configured before
    /// it's opened
    /// </summary>
    public virtual IDbConnection OpenReadOnlyDbConnection(string namedConnection, Action<IDbConnection> configure) =>
        OpenReadOnly(GetNamedConnection(namedConnection), namedConnection, configure);

    /// <summary>
    /// Open a read-only connection for queries that can read from a replica: its read replica when one is registered,
    /// otherwise this connection
    /// </summary>
    public virtual Task<IDbConnection> OpenReadOnlyDbConnectionAsync(CancellationToken token = default) =>
        this.OpenReadOnlyDbConnectionAsync((Action<IDbConnection>)null, token);

    /// <summary>
    /// Open a read-only connection for queries that can read from a replica, configured before it's opened
    /// </summary>
    public virtual Task<IDbConnection> OpenReadOnlyDbConnectionAsync(Action<IDbConnection> configure,
        CancellationToken token = default) => OpenReadOnlyAsync(this, null, configure, token);

    /// <summary>
    /// Open a read-only connection for queries that can read from a replica: the named connection's read replica when
    /// one is registered, otherwise the named connection
    /// </summary>
    public virtual Task<IDbConnection> OpenReadOnlyDbConnectionAsync(string namedConnection, CancellationToken token = default) =>
        this.OpenReadOnlyDbConnectionAsync(namedConnection, null, token);

    /// <summary>
    /// Open a read-only connection of a named connection for queries that can read from a replica, configured before
    /// it's opened
    /// </summary>
    public virtual Task<IDbConnection> OpenReadOnlyDbConnectionAsync(string namedConnection, Action<IDbConnection> configure,
        CancellationToken token = default) =>
        OpenReadOnlyAsync(GetNamedConnection(namedConnection), namedConnection, configure, token);
}

/// <summary>
/// Class OrmLiteConnectionFactoryExtensions.
/// </summary>
public static class OrmLiteConnectionFactoryExtensions
{
    /// <param name="connectionFactory">The connection factory.</param>
    extension(IDbConnectionFactory connectionFactory)
    {
        /// <summary>
        /// Alias for <see cref="OpenDbConnection(ServiceStack.Data.IDbConnectionFactory,string)" />
        /// </summary>
        /// <returns>IDbConnection.</returns>
        public IDbConnection Open()
        {
            return connectionFactory.OpenDbConnection();
        }

        public IDbConnection Open(Action<IDbConnection> configure)
        {
            var db = connectionFactory.CreateDbConnection();
            configure?.Invoke(db);
            db.Open();
            return db;
        }

        /// <summary>
        /// Alias for OpenDbConnectionAsync
        /// </summary>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;IDbConnection&gt;.</returns>
        public Task<IDbConnection> OpenDbConnectionAsync(CancellationToken token = default)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnectionAsync(token);
        }

        public Task<IDbConnection> OpenAsync(Action<IDbConnection> configure, CancellationToken token = default)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnectionAsync(configure, token);
        }

        public Task<IDbConnection> OpenAsync(string namedConnection, Action<IDbConnection> configure, CancellationToken token = default)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnectionAsync(namedConnection, configure, token);
        }

        /// <summary>
        /// Opens the asynchronous.
        /// </summary>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;IDbConnection&gt;.</returns>
        public Task<IDbConnection> OpenAsync(CancellationToken token = default)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnectionAsync(token);
        }

        /// <summary>
        /// Alias for OpenDbConnectionAsync
        /// </summary>
        /// <param name="namedConnection">The named connection.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;IDbConnection&gt;.</returns>
        public Task<IDbConnection> OpenAsync(string namedConnection, CancellationToken token = default)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnectionAsync(namedConnection, token);
        }

        /// <summary>
        /// Alias for OpenDbConnection
        /// </summary>
        /// <param name="namedConnection">The named connection.</param>
        /// <returns>IDbConnection.</returns>
        public IDbConnection Open(string namedConnection)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnection(namedConnection);
        }

        public IDbConnection OpenDbConnection(Action<IDbConnection> configure)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnection(configure);
        }

        /// <summary>
        /// Alias for OpenDbConnection
        /// </summary>
        /// <param name="namedConnection">The named connection.</param>
        /// <returns>IDbConnection.</returns>
        public IDbConnection OpenDbConnection(string namedConnection)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnection(namedConnection);
        }

        public IDbConnection OpenDbConnection(string namedConnection, Action<IDbConnection> configure)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnection(namedConnection, configure);
        }

        /// <summary>
        /// Opens the database connection asynchronous.
        /// </summary>
        /// <param name="namedConnection">The named connection.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;IDbConnection&gt;.</returns>
        public Task<IDbConnection> OpenDbConnectionAsync(string namedConnection, CancellationToken token = default)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnectionAsync(namedConnection, token);
        }

        /// <summary>
        /// Alias for OpenDbConnection
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>IDbConnection.</returns>
        public IDbConnection OpenDbConnectionString(string connectionString)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnectionString(connectionString);
        }

        /// <summary>
        /// Opens the database connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns>IDbConnection.</returns>
        public IDbConnection OpenDbConnectionString(string connectionString, string providerName)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnectionString(connectionString, providerName);
        }

        /// <summary>
        /// Opens the database connection string asynchronous.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;IDbConnection&gt;.</returns>
        public Task<IDbConnection> OpenDbConnectionStringAsync(string connectionString, CancellationToken token = default)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnectionStringAsync(connectionString, token);
        }

        /// <summary>
        /// Opens the database connection string asynchronous.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;IDbConnection&gt;.</returns>
        public Task<IDbConnection> OpenDbConnectionStringAsync(string connectionString, string providerName, CancellationToken token = default)
        {
            return ((OrmLiteConnectionFactory)connectionFactory).OpenDbConnectionStringAsync(connectionString, providerName, token);
        }

        /// <summary>
        /// Gets the dialect provider.
        /// </summary>
        /// <param name="dbInfo">The database information.</param>
        /// <returns>IOrmLiteDialectProvider.</returns>
        public IOrmLiteDialectProvider GetDialectProvider(ConnectionInfo dbInfo)
        {
            return dbInfo != null
                ? GetDialectProvider(connectionFactory, providerName: dbInfo.ProviderName, namedConnection: dbInfo.NamedConnection)
                : ((OrmLiteConnectionFactory)connectionFactory).DialectProvider;
        }

        /// <summary>
        /// Gets the dialect provider.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="namedConnection">The named connection.</param>
        /// <returns>IOrmLiteDialectProvider.</returns>
        /// <exception cref="System.NotSupportedException">Dialect provider is not registered '{provider}'</exception>
        /// <exception cref="System.NotSupportedException">Named connection is not registered '{namedConnection}'</exception>
        public IOrmLiteDialectProvider GetDialectProvider(string providerName = null, string namedConnection = null)
        {
            var dbFactory = (OrmLiteConnectionFactory)connectionFactory;

            if (!string.IsNullOrEmpty(providerName))
            {
                return OrmLiteConnectionFactory.DialectProviders.TryGetValue(providerName, out var provider)
                    ? provider
                    : throw new NotSupportedException($"Dialect provider is not registered '{providerName}'");
            }

            if (!string.IsNullOrEmpty(namedConnection))
            {
                return OrmLiteConnectionFactory.NamedConnections.TryGetValue(namedConnection, out var namedFactory)
                    ? namedFactory.DialectProvider
                    : throw new NotSupportedException($"Named connection is not registered '{namedConnection}'");
            }

            return dbFactory.DialectProvider;
        }
    }


    /// <summary>
    /// Converts to dbconnection.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <returns>IDbConnection.</returns>
    public static IDbConnection ToDbConnection(this IDbConnection db)
    {
        return db is IHasDbConnection hasDb
                   ? hasDb.DbConnection.ToDbConnection()
                   : db;
    }

    /// <summary>
    /// Converts to dbcommand.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>IDbCommand.</returns>
    public static IDbCommand ToDbCommand(this IDbCommand dbCmd)
    {
        return dbCmd is IHasDbCommand hasDbCmd
                   ? hasDbCmd.DbCommand.ToDbCommand()
                   : dbCmd;
    }

    /// <summary>
    /// Converts to dbtransaction.
    /// </summary>
    /// <param name="dbTrans">The database trans.</param>
    /// <returns>IDbTransaction.</returns>
    public static IDbTransaction ToDbTransaction(this IDbTransaction dbTrans)
    {
        return dbTrans is IHasDbTransaction hasDbTrans
                   ? hasDbTrans.DbTransaction
                   : dbTrans;
    }

    /// <summary>
    /// Gets the connection identifier.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <returns>Guid.</returns>
    public static Guid GetConnectionId(this IDbConnection db)
    {
        return db is OrmLiteConnection conn ? conn.ConnectionId : Guid.Empty;
    }

    /// <summary>
    /// Gets the connection identifier.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>Guid.</returns>
    public static Guid GetConnectionId(this IDbCommand dbCmd)
    {
        return dbCmd is OrmLiteCommand cmd ? cmd.ConnectionId : Guid.Empty;
    }

    /// <param name="dbFactory">The database factory.</param>
    extension(IDbConnectionFactory dbFactory)
    {
        /// <summary>
        /// Registers the connection.
        /// </summary>
        /// <param name="namedConnection">The named connection.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dialectProvider">The dialect provider.</param>
        public void RegisterConnection(string namedConnection, string connectionString, IOrmLiteDialectProvider dialectProvider)
        {
            ((OrmLiteConnectionFactory)dbFactory).RegisterConnection(namedConnection, connectionString, dialectProvider);
        }

        /// <summary>
        /// Registers the connection.
        /// </summary>
        /// <param name="namedConnection">The named connection.</param>
        /// <param name="connectionFactory">The connection factory.</param>
        public void RegisterConnection(string namedConnection, OrmLiteConnectionFactory connectionFactory)
        {
            ((OrmLiteConnectionFactory)dbFactory).RegisterConnection(namedConnection, connectionFactory);
        }

        /// <summary>
        /// Opens the database connection.
        /// </summary>
        /// <param name="connInfo">The connection information.</param>
        /// <returns>IDbConnection.</returns>
        public IDbConnection OpenDbConnection(ConnectionInfo connInfo)
        {
            if (dbFactory is IDbConnectionFactoryExtended dbFactoryExt && connInfo != null)
            {
                if (connInfo.ConnectionString != null)
                {
                    return connInfo.ProviderName != null
                        ? dbFactoryExt.OpenDbConnectionString(connInfo.ConnectionString, connInfo.ProviderName)
                        : dbFactoryExt.OpenDbConnectionString(connInfo.ConnectionString);
                }

                if (connInfo.NamedConnection != null)
                {
                    return dbFactoryExt.OpenDbConnection(connInfo.NamedConnection);
                }
            }

            return dbFactory.Open();
        }

        /// <summary>
        /// Open database connection as an asynchronous operation.
        /// </summary>
        /// <param name="connInfo">The connection information.</param>
        /// <returns>A Task&lt;IDbConnection&gt; representing the asynchronous operation.</returns>
        public async Task<IDbConnection> OpenDbConnectionAsync(ConnectionInfo connInfo)
        {
            if (dbFactory is IDbConnectionFactoryExtended dbFactoryExt && connInfo != null)
            {
                if (connInfo.ConnectionString != null)
                {
                    return connInfo.ProviderName != null
                        ? await dbFactoryExt.OpenDbConnectionStringAsync(connInfo.ConnectionString, connInfo.ProviderName).ConfigAwait()
                        : await dbFactoryExt.OpenDbConnectionStringAsync(connInfo.ConnectionString).ConfigAwait();
                }

                if (connInfo.NamedConnection != null)
                {
                    return await dbFactoryExt.OpenDbConnectionAsync(connInfo.NamedConnection).ConfigAwait();
                }
            }

            return await dbFactory.OpenAsync().ConfigAwait();
        }

        /// <summary>
        /// Register a read replica of the main connection, which OpenReadOnlyDbConnection() opens
        /// </summary>
        /// <param name="connectionString">The connection string of the read replica.</param>
        /// <param name="fallbackToPrimary">Open the main connection when the read replica can't be opened.</param>
        public void RegisterReadReplica(string connectionString,
            bool fallbackToPrimary = false) =>
            ((OrmLiteConnectionFactory)dbFactory).RegisterReadReplica(connectionString, fallbackToPrimary);

        /// <summary>
        /// Register a read replica of a named connection, which OpenReadOnlyDbConnection(namedConnection) opens
        /// </summary>
        /// <param name="namedConnection">The named connection.</param>
        /// <param name="connectionString">The connection string of the read replica.</param>
        /// <param name="fallbackToPrimary">Open the named connection when the read replica can't be opened.</param>
        public void RegisterReadReplica(string namedConnection,
            string connectionString, bool fallbackToPrimary = false) =>
            ((OrmLiteConnectionFactory)dbFactory).RegisterReadReplica(namedConnection, connectionString, fallbackToPrimary);

        /// <summary>
        /// Open a connection for queries that can read from a replica: the read replica when one is registered,
        /// otherwise the main connection, e.g:
        /// <para>using var db = dbFactory.OpenReadOnlyDbConnection();</para>
        /// </summary>
        /// <returns>IDbConnection.</returns>
        public IDbConnection OpenReadOnlyDbConnection() =>
            dbFactory is IDbReadOnlyConnectionFactory factory ? factory.OpenReadOnlyDbConnection(configure: null) : dbFactory.OpenDbConnection();

        /// <summary>
        /// Open a connection for queries that can read from a replica: the named connection's read replica when one is
        /// registered, otherwise the named connection
        /// </summary>
        /// <param name="namedConnection">The named connection.</param>
        /// <returns>IDbConnection.</returns>
        public IDbConnection OpenReadOnlyDbConnection(string namedConnection) =>
            dbFactory is IDbReadOnlyConnectionFactory factory
                ? factory.OpenReadOnlyDbConnection(namedConnection, null)
                : dbFactory.OpenDbConnection(namedConnection);

        /// <summary>
        /// Open a connection for queries that can read from a replica: the read replica when one is registered,
        /// otherwise the main connection
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A Task&lt;IDbConnection&gt; representing the asynchronous operation.</returns>
        public Task<IDbConnection> OpenReadOnlyDbConnectionAsync(CancellationToken token = default) => dbFactory is IDbReadOnlyConnectionFactory factory
            ? factory.OpenReadOnlyDbConnectionAsync(configure: null, token)
            : dbFactory.OpenDbConnectionAsync(token);

        /// <summary>
        /// Open a connection for queries that can read from a replica: the named connection's read replica when one is
        /// registered, otherwise the named connection
        /// </summary>
        /// <param name="namedConnection">The named connection.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A Task&lt;IDbConnection&gt; representing the asynchronous operation.</returns>
        public Task<IDbConnection> OpenReadOnlyDbConnectionAsync(string namedConnection, CancellationToken token = default) => dbFactory is IDbReadOnlyConnectionFactory factory
            ? factory.OpenReadOnlyDbConnectionAsync(namedConnection, null, token)
            : dbFactory.OpenDbConnectionAsync(namedConnection, token);
    }
}