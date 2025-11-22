#nullable enable
using System;

using ServiceStack.OrmLite.SqlServer;

namespace ServiceStack.OrmLite;

public static class SqlServerConfiguration
{
    public static SqlServerOrmLiteDialectProvider Configure(SqlServerOrmLiteDialectProvider dialect)
    {
        // New defaults for new Apps
        dialect.UseJson = true;
        return dialect;
    }

    extension(OrmLiteConfigOptions config)
    {
        /// <summary>
        /// Configure to use the latest version of SQL Server
        /// </summary>
        public SqlServer2022OrmLiteDialectProvider UseSqlServer(string? connectionString, Action<SqlServer2022OrmLiteDialectProvider>? configure=null)
        {
            ArgumentNullException.ThrowIfNull(connectionString);

            var dialect = (SqlServer2022OrmLiteDialectProvider)Configure(new SqlServer2022OrmLiteDialectProvider());
            configure?.Invoke(dialect);
            config.Init(connectionString, dialect);
            return dialect;
        }

        /// <summary>
        /// Configure to use the latest version of SQL Server
        /// </summary>
        public TVersion UseSqlServer<TVersion>(string? connectionString, Action<TVersion>? configure=null)
            where TVersion : SqlServerOrmLiteDialectProvider, new()
        {
            ArgumentNullException.ThrowIfNull(connectionString);

            var dialect = (TVersion)Configure(new TVersion());
            configure?.Invoke(dialect);
            config.Init(connectionString, dialect);
            return dialect;
        }
    }

    extension(OrmLiteConfigurationBuilder builder)
    {
        /// <summary>
        /// Add a connection to the latest version of SQL Server
        /// </summary>
        public OrmLiteConfigurationBuilder AddSqlServer(string namedConnection, string? connectionString, Action<SqlServer2022OrmLiteDialectProvider>? configure=null)
        {
            ArgumentNullException.ThrowIfNull(connectionString);

            var dialect = (SqlServer2022OrmLiteDialectProvider)Configure(new SqlServer2022OrmLiteDialectProvider());
            configure?.Invoke(dialect);
            builder.DbFactory.RegisterConnection(namedConnection, connectionString, dialect);
            return builder;
        }

        public OrmLiteConfigurationBuilder AddSqlServer<TVersion>(string namedConnection, string? connectionString, Action<TVersion>? configure=null)
            where TVersion : SqlServerOrmLiteDialectProvider, new()
        {
            ArgumentNullException.ThrowIfNull(connectionString);

            var dialect = (TVersion)Configure(new TVersion());
            configure?.Invoke(dialect);
            builder.DbFactory.RegisterConnection(namedConnection, connectionString, dialect);
            return builder;
        }
    }
}
