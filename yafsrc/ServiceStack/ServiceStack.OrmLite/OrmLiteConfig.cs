// ***********************************************************************
// <copyright file="OrmLiteConfig.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using ServiceStack.Logging;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteConfig.
/// </summary>
public static class OrmLiteConfig
{
    /// <summary>
    /// The identifier field
    /// </summary>
    public const string IdField = "Id";

    /// <summary>
    /// The default command timeout
    /// </summary>
    private const int DefaultCommandTimeout = 30;

    /// <summary>
    /// The command timeout
    /// </summary>
    private static int? commandTimeout;

    /// <summary>
    /// Gets or sets the wait time before terminating the attempt to execute a command and generating an error (in seconds).
    /// </summary>
    public static int CommandTimeout
    {
        get => commandTimeout ?? DefaultCommandTimeout;
        set => commandTimeout = value;
    }

    /// <summary>
    /// The dialect provider
    /// </summary>
    private static IOrmLiteDialectProvider dialectProvider;

    /// <summary>
    /// Gets or sets the dialect provider.
    /// </summary>
    /// <value>The dialect provider.</value>
    /// <exception cref="System.ArgumentNullException">DialectProvider - You must set the singleton 'OrmLiteConfig.DialectProvider' to use the OrmLiteWriteExtensions</exception>
    public static IOrmLiteDialectProvider DialectProvider
    {
        get
        {
            if (dialectProvider == null)
            {
                throw new ArgumentNullException(nameof(DialectProvider),
                    "You must set the singleton 'OrmLiteConfig.DialectProvider' to use the OrmLiteWriteExtensions");
            }

            return dialectProvider;
        }
        set => dialectProvider = value;
    }

    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Gets the dialect provider.
        /// </summary>
        /// <returns>IOrmLiteDialectProvider.</returns>
        public IOrmLiteDialectProvider GetDialectProvider()
        {
            return dbCmd is IHasDialectProvider hasDialectProvider
                ? hasDialectProvider.DialectProvider
                : DialectProvider;
        }

        /// <summary>
        /// Dialects the specified database command.
        /// </summary>
        /// <returns>IOrmLiteDialectProvider.</returns>
        public IOrmLiteDialectProvider Dialect()
        {
            return dbCmd is IHasDialectProvider hasDialectProvider
                ? hasDialectProvider.DialectProvider
                : DialectProvider;
        }
    }

    /// <param name="db">The database.</param>
    extension(IDbConnection db)
    {
        /// <summary>
        /// Gets the dialect provider.
        /// </summary>
        /// <returns>IOrmLiteDialectProvider.</returns>
        public IOrmLiteDialectProvider GetDialectProvider()
        {
            return db is IHasDialectProvider hasDialectProvider
                ? hasDialectProvider.DialectProvider
                : DialectProvider;
        }

        /// <summary>
        /// Gets the naming strategy.
        /// </summary>
        /// <returns>INamingStrategy.</returns>
        public INamingStrategy GetNamingStrategy()
        {
            return db.GetDialectProvider().NamingStrategy;
        }

        /// <summary>
        /// Dialects the specified database.
        /// </summary>
        /// <returns>IOrmLiteDialectProvider.</returns>
        public IOrmLiteDialectProvider Dialect()
        {
            return db is IHasDialectProvider hasDialectProvider
                ? hasDialectProvider.DialectProvider
                : DialectProvider;
        }
    }

    /// <summary>
    /// Gets the execute filter.
    /// </summary>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <returns>IOrmLiteExecFilter.</returns>
    public static IOrmLiteExecFilter GetExecFilter(this IOrmLiteDialectProvider dialectProvider)
    {
        return dialectProvider != null
                   ? dialectProvider.ExecFilter ?? ExecFilter
                   : ExecFilter;
    }

    /// <summary>
    /// Gets the execute filter.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>IOrmLiteExecFilter.</returns>
    public static IOrmLiteExecFilter GetExecFilter(this IDbCommand dbCmd)
    {
        var dialect = dbCmd is IHasDialectProvider hasDialectProvider
                          ? hasDialectProvider.DialectProvider
                          : DialectProvider;
        return dialect.GetExecFilter();
    }

    /// <param name="db">The database.</param>
    extension(IDbConnection db)
    {
        /// <summary>
        /// Gets the execute filter.
        /// </summary>
        /// <returns>IOrmLiteExecFilter.</returns>
        public IOrmLiteExecFilter GetExecFilter()
        {
            var dialect = db is IHasDialectProvider hasDialectProvider
                ? hasDialectProvider.DialectProvider
                : DialectProvider;
            return dialect.GetExecFilter();
        }

        /// <summary>
        /// Sets the last command text.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        public void SetLastCommandText(string sql)
        {
            if (db is OrmLiteConnection ormLiteConn)
            {
                ormLiteConn.LastCommandText = sql;
            }
        }

        /// <summary>
        /// Sets the last command.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        public void SetLastCommand(IDbCommand dbCmd)
        {
            if (db is OrmLiteConnection ormLiteConn)
            {
                ormLiteConn.LastCommand = dbCmd;
            }
        }

        public IDbConnection WithTag(string name)
        {
            if (db is OrmLiteConnection ormLiteConn)
            {
                ormLiteConn.Tag = name;
            }

            return db;
        }

        public string GetTag()
        {
            return db is OrmLiteConnection ormLiteConn ? ormLiteConn.Tag : null;
        }
    }

    extension(IDbCommand db)
    {
        public string GetTag()
        {
            return db is OrmLiteCommand ormLiteCmd
                ? ormLiteCmd.OrmLiteConnection.GetTag()
                : null;
        }

        public TimeSpan? GetElapsedTime()
        {
            return db is OrmLiteCommand ormLiteCmd
                ? ormLiteCmd.GetElapsedTime()
                : null;
        }
    }

    /// <summary>
    /// The requires orm lite connection
    /// </summary>
    private const string RequiresOrmLiteConnection = "{0} can only be set on a OrmLiteConnectionFactory connection, not a plain IDbConnection";

    /// <param name="db"></param>
    extension(IDbConnection db)
    {
        /// <summary>
        /// Sets the wait time before terminating the attempt to execute a command and generating an error.
        /// </summary>
        /// <param name="commandTimeout">Command execution timeout(in seconds)</param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetCommandTimeout(int? commandTimeout)
        {
            if (db is not OrmLiteConnection ormLiteConn)
            {
                throw new NotImplementedException(string.Format(RequiresOrmLiteConnection, nameof(CommandTimeout)));
            }

            ormLiteConn.CommandTimeout = commandTimeout;
        }

        /// <summary>
        /// <inheritdoc cref="SetCommandTimeout(IDbConnection,int?)"/>
        /// </summary>
        /// <param name="commandTimeout">Command execution timeout</param>
        public void SetCommandTimeout(TimeSpan? commandTimeout)
        {
            SetCommandTimeout(db, (int?)commandTimeout?.TotalSeconds);
        }
    }

    /// <param name="dbConnectionStringOrFilePath">The database connection string or file path.</param>
    extension(string dbConnectionStringOrFilePath)
    {
        /// <summary>
        /// Converts to dbconnection.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        public IDbConnection ToDbConnection()
        {
            return dbConnectionStringOrFilePath.ToDbConnection(DialectProvider);
        }

        /// <summary>
        /// Opens the database connection.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        public IDbConnection OpenDbConnection()
        {
            var sqlConn = dbConnectionStringOrFilePath.ToDbConnection(DialectProvider);
            sqlConn.Open();
            return sqlConn;
        }

        /// <summary>
        /// Opens the read only database connection.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        public IDbConnection OpenReadOnlyDbConnection()
        {
            var options = new Dictionary<string, string> { { "Read Only", "True" } };

            var dbConn = DialectProvider.CreateConnection(dbConnectionStringOrFilePath, options);
            dbConn.Open();
            return dbConn;
        }
    }

    /// <summary>
    /// Clears the cache.
    /// </summary>
    public static void ClearCache()
    {
        OrmLiteConfigExtensions.ClearCache();
    }

    /// <summary>
    /// Gets the model metadata.
    /// </summary>
    /// <param name="modelType">Type of the model.</param>
    /// <returns>ModelDefinition.</returns>
    public static ModelDefinition GetModelMetadata(this Type modelType)
    {
        return modelType.GetModelDefinition();
    }

    /// <summary>
    /// Converts to dbconnection.
    /// </summary>
    /// <param name="dbConnectionStringOrFilePath">The database connection string or file path.</param>
    /// <param name="dialectProvider">The dialect provider.</param>
    /// <returns>IDbConnection.</returns>
    public static IDbConnection ToDbConnection(this string dbConnectionStringOrFilePath, IOrmLiteDialectProvider dialectProvider)
    {
        var dbConn = dialectProvider.CreateConnection(dbConnectionStringOrFilePath, options: null);
        return dbConn;
    }

    /// <summary>
    /// Resets the log factory.
    /// </summary>
    /// <param name="logFactory">The log factory.</param>
    public static void ResetLogFactory(ILogFactory logFactory = null)
    {
        logFactory ??= LogManager.LogFactory;
        LogManager.LogFactory = logFactory;
        OrmLiteLog.Log = logFactory.GetLogger(typeof(OrmLiteLog));
    }

    /// <summary>
    /// Gets or sets a value indicating whether [disable column guess fallback].
    /// </summary>
    /// <value><c>true</c> if [disable column guess fallback]; otherwise, <c>false</c>.</value>
    public static bool DisableColumnGuessFallback { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [strip upper in like].
    /// </summary>
    /// <value><c>true</c> if [strip upper in like]; otherwise, <c>false</c>.</value>
    public static bool StripUpperInLike { get; set; }
#if NET10_0_OR_GREATER
            = true;
#endif

    /// <summary>
    /// Gets or sets the results filter.
    /// </summary>
    /// <value>The results filter.</value>
    public static IOrmLiteResultsFilter ResultsFilter
    {
        get
        {
            var state = OrmLiteContext.OrmLiteState;
            return state?.ResultsFilter;
        }
        set => OrmLiteContext.GetOrCreateState().ResultsFilter = value;
    }

    /// <summary>
    /// Gets or sets the execute filter.
    /// </summary>
    /// <value>The execute filter.</value>
    public static IOrmLiteExecFilter ExecFilter {
        get {
            field ??= new OrmLiteExecFilter();

            return dialectProvider != null
                ? dialectProvider.ExecFilter ?? field
                : field;
        }
        set;
    }

    /// <summary>
    /// Gets or sets the before execute filter.
    /// </summary>
    /// <value>The before execute filter.</value>
    public static Action<IDbCommand> BeforeExecFilter { get; set; }
    /// <summary>
    /// Gets or sets the after execute filter.
    /// </summary>
    /// <value>The after execute filter.</value>
    public static Action<IDbCommand> AfterExecFilter { get; set; }

    /// <summary>
    /// Gets or sets the insert filter.
    /// </summary>
    /// <value>The insert filter.</value>
    public static Action<IDbCommand, object> InsertFilter { get; set; }
    /// <summary>
    /// Gets or sets the update filter.
    /// </summary>
    /// <value>The update filter.</value>
    public static Action<IDbCommand, object> UpdateFilter { get; set; }
    /// <summary>
    /// Gets or sets the SQL expression select filter.
    /// </summary>
    /// <value>The SQL expression select filter.</value>
    public static Action<IUntypedSqlExpression> SqlExpressionSelectFilter { get; set; }
    /// <summary>
    /// Gets or sets the load reference select filter.
    /// </summary>
    /// <value>The load reference select filter.</value>
    public static Func<Type, string, string> LoadReferenceSelectFilter { get; set; }


    /// <summary>
    /// Gets or sets the string filter.
    /// </summary>
    /// <value>The string filter.</value>
    public static Func<string, string> StringFilter { get; set; }

    /// <summary>
    /// Gets or sets the on database null filter.
    /// </summary>
    /// <value>The on database null filter.</value>
    public static Func<FieldDefinition, object> OnDbNullFilter { get; set; }

    /// <summary>
    /// Gets or sets the populated object filter.
    /// </summary>
    /// <value>The populated object filter.</value>
    public static Action<object> PopulatedObjectFilter { get; set; }

    /// <summary>
    /// Gets or sets the exception filter.
    /// </summary>
    /// <value>The exception filter.</value>
    public static Action<IDbCommand, Exception> ExceptionFilter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [throw on error].
    /// </summary>
    /// <value><c>true</c> if [throw on error]; otherwise, <c>false</c>.</value>
    public static bool ThrowOnError { get; set; } = true;

    /// <summary>
    /// The sanitize field name for parameter name function
    /// </summary>
    public static Func<string, string> SanitizeFieldNameForParamNameFn = fieldName =>
        (fieldName ?? "").Replace(" ", "");

    /// <summary>
    /// Gets or sets a value indicating whether this instance is case insensitive.
    /// </summary>
    /// <value><c>true</c> if this instance is case insensitive; otherwise, <c>false</c>.</value>
    public static bool IsCaseInsensitive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [deoptimize reader].
    /// </summary>
    /// <value><c>true</c> if [deoptimize reader]; otherwise, <c>false</c>.</value>
    public static bool DeoptimizeReader { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [skip foreign keys].
    /// </summary>
    /// <value><c>true</c> if [skip foreign keys]; otherwise, <c>false</c>.</value>
    public static bool SkipForeignKeys { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [include table prefixes].
    /// </summary>
    /// <value><c>true</c> if [include table prefixes]; otherwise, <c>false</c>.</value>
    public static bool IncludeTablePrefixes { get; set; }

    /// <summary>
    /// Gets or sets the SQL expression initialize filter.
    /// </summary>
    /// <value>The SQL expression initialize filter.</value>
    public static Action<IUntypedSqlExpression> SqlExpressionInitFilter { get; set; }

    /// <summary>
    /// Gets or sets the parameter name filter.
    /// </summary>
    /// <value>The parameter name filter.</value>
    public static Func<string, string> ParamNameFilter { get; set; }

    /// <summary>
    /// Gets or sets the on model definition initialize.
    /// </summary>
    /// <value>The on model definition initialize.</value>
    public static Action<ModelDefinition> OnModelDefinitionInit { get; set; }
}