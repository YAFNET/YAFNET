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
public class OrmLiteConnectionFactory : IDbConnectionFactoryExtended
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
                             : this.OrmLiteConnection;

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
                                       : factory.OrmLiteConnection;
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
        var connection = this.CreateDbConnection();
        connection.Open();

        return connection;
    }
    public virtual IDbConnection OpenDbConnection(Action<IDbConnection> configure)
    {
        var connection = this.CreateDbConnection();
        configure(connection);
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Open database connection as an asynchronous operation.
    /// </summary>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;IDbConnection&gt; representing the asynchronous operation.</returns>
    public async virtual Task<IDbConnection> OpenDbConnectionAsync(CancellationToken token = default)
    {
        var connection = this.CreateDbConnection();
        if (connection is OrmLiteConnection ormliteConn)
        {
            await ormliteConn.OpenAsync(token).ConfigAwait();
            return connection;
        }

        await this.DialectProvider.OpenAsync(connection, token).ConfigAwait();
        return connection;
    }

    public async virtual Task<IDbConnection> OpenDbConnectionAsync(Action<IDbConnection> configure, CancellationToken token = default)
    {
        var connection = this.CreateDbConnection();
        configure(connection);
        if (connection is OrmLiteConnection ormliteConn)
        {
            await ormliteConn.OpenAsync(token).ConfigAwait();
            return connection;
        }

        await this.DialectProvider.OpenAsync(connection, token).ConfigAwait();
        return connection;
    }

    /// <summary>
    /// Open database connection as an asynchronous operation.
    /// </summary>
    /// <param name="namedConnection">The named connection.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;IDbConnection&gt; representing the asynchronous operation.</returns>
    public async virtual Task<IDbConnection> OpenDbConnectionAsync(string namedConnection, CancellationToken token = default)
    {
        var connection = CreateDbConnection(namedConnection);
        if (connection is OrmLiteConnection ormliteConn)
        {
            await ormliteConn.OpenAsync(token).ConfigAwait();
            return connection;
        }

        await this.DialectProvider.OpenAsync(connection, token).ConfigAwait();
        return connection;
    }
    public async virtual Task<IDbConnection> OpenDbConnectionAsync(string namedConnection, Action<IDbConnection> configure, CancellationToken token = default)
    {
        var connection = CreateDbConnection(namedConnection);
        configure(connection);
        if (connection is OrmLiteConnection ormliteConn)
        {
            await ormliteConn.OpenAsync(token).ConfigAwait();
            return connection;
        }

        await this.DialectProvider.OpenAsync(connection, token).ConfigAwait();
        return connection;
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

        connection.Open();

        return connection;
    }

    public virtual IDbConnection OpenDbConnectionString(string connectionString, Action<IDbConnection> configure)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        var connection = this.DialectProvider.CreateOrmLiteConnection(this);
        connection.ConnectionString = connectionString;
        configure(connection);

        connection.Open();
        return connection;
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

        await connection.OpenAsync(token).ConfigAwait();

        return connection;
    }

    public async virtual Task<IDbConnection> OpenDbConnectionStringAsync(string connectionString, Action<IDbConnection> configure, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        var connection = this.DialectProvider.CreateOrmLiteConnection(this);
        connection.ConnectionString = connectionString;
        configure(connection);

        await connection.OpenAsync(token).ConfigAwait();
        return connection;
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
        var connection = CreateDbConnection(namedConnection);

        connection.Open();

        return connection;
    }
    public virtual IDbConnection OpenDbConnection(string namedConnection, Action<IDbConnection> configure)
    {
        var connection = CreateDbConnection(namedConnection);
        configure(connection);
        connection.Open();
        return connection;
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
    }
}