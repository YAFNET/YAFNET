// ***********************************************************************
// <copyright file="Migrator.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************


#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

using ServiceStack.Data;
using ServiceStack.DataAnnotations;
using ServiceStack.Logging;
using ServiceStack.OrmLite.Base.Common;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class Migration.
/// Implements the <see cref="ServiceStack.IMeta" />
/// </summary>
/// <seealso cref="ServiceStack.IMeta" />
public class Migration : IMeta
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    [AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the created date.
    /// </summary>
    /// <value>The created date.</value>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the completed date.
    /// </summary>
    /// <value>The completed date.</value>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    /// <value>The connection string.</value>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the named connection.
    /// </summary>
    /// <value>The named connection.</value>
    public string? NamedConnection { get; set; }

    /// <summary>
    /// Gets or sets the log.
    /// </summary>
    /// <value>The log.</value>
    [StringLength(StringLengthAttribute.MaxText)]
    public string? Log { get; set; }

    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    /// <value>The error code.</value>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    /// <value>The error message.</value>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the error stack trace.
    /// </summary>
    /// <value>The error stack trace.</value>
    [StringLength(StringLengthAttribute.MaxText)]
    public string? ErrorStackTrace { get; set; }

    /// <summary>
    /// Gets or sets the meta.
    /// </summary>
    /// <value>The meta.</value>
    [StringLength(StringLengthAttribute.MaxText)]
    public Dictionary<string, string>? Meta { get; set; }
}

/// <summary>
/// Class MigrationBase.
/// </summary>
/// <seealso cref="ServiceStack.IAppTask" />
public abstract class MigrationBase : IAppTask
{
    /// <summary>
    /// Gets or sets the database factory.
    /// </summary>
    /// <value>The database factory.</value>
    public IDbConnectionFactory? DbFactory { get; set; }

    /// <summary>
    /// Gets or sets the database.
    /// </summary>
    /// <value>The database.</value>
    public IDbConnection? Db { get; set; }

    /// <summary>
    /// Gets or sets the transaction.
    /// </summary>
    /// <value>The transaction.</value>
    public IDbTransaction? Transaction { get; set; }

    /// <summary>
    /// Gets or sets the log.
    /// </summary>
    /// <value>The log.</value>
    public string? Log { get; set; }

    /// <summary>
    /// Gets or sets the started at.
    /// </summary>
    /// <value>The started at.</value>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the completed date.
    /// </summary>
    /// <value>The completed date.</value>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Gets or sets the error.
    /// </summary>
    /// <value>The error.</value>
    public Exception? Error { get; set; }

    /// <summary>
    /// Add additional logs to capture in Migration table
    /// </summary>
    /// <value>The migration log.</value>
    public StringBuilder MigrationLog { get; set; } = new();

    /// <summary>
    /// Afters the open.
    /// </summary>
    public virtual void AfterOpen() { }

    /// <summary>
    /// Before the commit.
    /// </summary>
    public virtual void BeforeCommit() { }

    /// <summary>
    /// Before the rollback.
    /// </summary>
    public virtual void BeforeRollback() { }

    /// <summary>
    /// Db Migrations.
    /// </summary>
    public virtual void Up() { }

    /// <summary>
    /// Revert Db Migrations.
    /// </summary>
    public virtual void Down() { }
}

/// <summary>
/// Class Migrator.
/// </summary>
public class Migrator
{
    /// <summary>
    /// All
    /// </summary>
    public const string All = "all";

    /// <summary>
    /// The last
    /// </summary>
    public const string Last = "last";

    /// <summary>
    /// Gets the database factory.
    /// </summary>
    /// <value>The database factory.</value>
    public IDbConnectionFactory DbFactory { get; }

    /// <summary>
    /// Gets the migration types.
    /// </summary>
    /// <value>The migration types.</value>
    public Type[] MigrationTypes { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Migrator"/> class.
    /// </summary>
    /// <param name="dbFactory">The database factory.</param>
    /// <param name="migrationAssemblies">The migration assemblies.</param>
    public Migrator(IDbConnectionFactory dbFactory, params Assembly[] migrationAssemblies)
        : this(dbFactory, GetAllMigrationTypes(migrationAssemblies).ToArray())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Migrator"/> class.
    /// </summary>
    /// <param name="dbFactory">The database factory.</param>
    /// <param name="migrationTypes">The migration types.</param>
    public Migrator(IDbConnectionFactory dbFactory, params Type[] migrationTypes)
    {
        this.DbFactory = dbFactory;
        this.MigrationTypes = migrationTypes;
        JsConfig.InitStatics();
    }

    /// <summary>
    /// Gets or sets the timeout.
    /// </summary>
    /// <value>The timeout.</value>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the log.
    /// </summary>
    /// <value>The log.</value>
    public ILog Log { get; set; } = new ConsoleLogger(typeof(Migrator));

    /// <summary>
    /// Gets the next migration to run.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="migrationTypes">The migration types.</param>
    /// <returns>System.Type?.</returns>
    private Type? GetNextMigrationToRun(IDbConnection db, List<Type> migrationTypes)
    {
        var completedMigrations = new List<Type>();

        var q = db.From<Migration>()
            .OrderByDescending(x => x.Name);
        // Select all previously run migrations and find the last completed migration
        var runMigrations = db.Select(q);
        var lastRun = runMigrations.FirstOrDefault();
        if (lastRun != null)
        {
            var elapsedTime = DateTime.UtcNow - lastRun.CreatedDate;
            if (lastRun.CompletedDate == null)
            {
                if (elapsedTime < this.Timeout)
                {
                    throw new InfoException(
                        $"Migration '{lastRun.Name}' is still in progress, timeout in {(this.Timeout - elapsedTime).TotalSeconds:N3}s.");
                }

                this.Log.Info(
                    $"Migration '{lastRun.Name}' failed to complete {elapsedTime.TotalSeconds:N3}s ago, re-running...");
                db.DeleteById<Migration>(lastRun.Id);
            }

            // Re-run last migration
            if (lastRun.CompletedDate == null)
            {
                foreach (var migrationType in migrationTypes)
                {
                    if (migrationType.Name != lastRun.Name)
                    {
                        completedMigrations.Add(migrationType);
                    }
                    else
                    {
                        migrationTypes.RemoveAll(x => completedMigrations.Contains(x));
                        return migrationType;
                    }
                }

                return null;
            }

            // Remove completed migrations
            completedMigrations = migrationTypes.Any(x => x.Name == lastRun.Name)
                ? migrationTypes.TakeWhile(x => x.Name != lastRun.Name).ToList()
                : [];

            // Make sure we don't rerun any migrations that have already been run
            foreach (var migrationType in migrationTypes)
            {
                if (runMigrations.Any(x => x.Name == migrationType.Name && x.CompletedDate != null))
                {
                    completedMigrations.Add(migrationType);
                }
            }

            if (completedMigrations.Count > 0)
            {
                migrationTypes.RemoveAll(x => completedMigrations.Contains(x));
            }

            var nextMigration = migrationTypes.FirstOrDefault();
            if (nextMigration == null)
            {
                return null;
            }

            // Remove completed migration
            if (nextMigration.Name == lastRun.Name)
            {
                migrationTypes.Remove(nextMigration);
            }
        }

        // Return next migration
        return migrationTypes.FirstOrDefault();
    }

    /// <summary>
    /// Runs this instance.
    /// </summary>
    /// <returns>ServiceStack.AppTaskResult.</returns>
    public AppTaskResult Run()
    {
        return this.Run(throwIfError: true);
    }

    /// <summary>
    /// Runs the specified throw if error.
    /// </summary>
    /// <param name="throwIfError">The throw if error.</param>
    /// <returns>ServiceStack.AppTaskResult.</returns>
    public AppTaskResult Run(bool throwIfError)
    {
        using var db = this.DbFactory.Open();
        Init(db);

        var remainingMigrations = this.MigrationTypes.ToList();

        this.LogMigrationsFound(remainingMigrations);

        var startAt = DateTime.UtcNow;
        var migrationsRun = new List<IAppTask>();

        while (true)
        {
            Type? nextRun;
            try
            {
                nextRun = this.GetNextMigrationToRun(db, remainingMigrations);
                if (nextRun == null)
                {
                    break;
                }
            }
            catch (Exception e)
            {
                this.Log.Error(e.Message);
                if (throwIfError)
                {
                    throw;
                }

                return new AppTaskResult(migrationsRun) { Error = e };
            }

            var migrationStartAt = DateTime.UtcNow;

            var descFmt = AppTasks.GetDescFmt(nextRun);
            var namedConnections =
                nextRun.AllAttributes<NamedConnectionAttribute>().Select(x => x.Name ?? null).ToArray();
            if (namedConnections.Length == 0)
            {
                namedConnections = [null];
            }

            foreach (var namedConnection in namedConnections)
            {
                var namedDesc = namedConnection == null ? "" : $" ({namedConnection})";
                this.Log.Info($"Running {nextRun.Name}{descFmt}{namedDesc}...");

                var migration = new Migration {
                    Name = nextRun.Name,
                    Description = AppTasks.GetDesc(nextRun),
                    CreatedDate = DateTime.UtcNow,
                    ConnectionString =
                        OrmLiteUtils.MaskPassword(((OrmLiteConnectionFactory)this.DbFactory).ConnectionString),
                    NamedConnection = namedConnection
                };
                var id = db.Insert(migration, selectIdentity: true);

                var instance = Run(this.DbFactory, nextRun, x => x.Up(), namedConnection);
                migrationsRun.Add(instance);
                this.Log.Info(instance.Log);

                if (instance.Error == null)
                {
                    this.Log.Info(
                        $"Completed {nextRun.Name}{descFmt} in {(DateTime.UtcNow - migrationStartAt).TotalSeconds:N3}s" +
                        Environment.NewLine);

                    // Record completed migration run in DB
                    db.UpdateOnly(() => new Migration {
                        Log = instance.Log,
                        CompletedDate = DateTime.UtcNow
                    }, where: x => x.Id == id);
                    remainingMigrations.Remove(nextRun);
                }
                else
                {
                    var e = instance.Error;
                    this.Log.Error(e.Message, e);

                    // Save Error in DB
                    db.UpdateOnly(() => new Migration {
                        Log = instance.Log,
                        ErrorCode = e.GetType().Name,
                        ErrorMessage = e.Message,
                        ErrorStackTrace = e.StackTrace
                    }, where: x => x.Id == id);

                    if (throwIfError)
                    {
                        throw instance.Error;
                    }

                    return new AppTaskResult(migrationsRun);
                }
            }
        }

        var migrationsCompleted = migrationsRun.Count(x => x.Error == null);
        if (migrationsCompleted == 0)
        {
            this.Log.Info("No migrations to run.");
        }
        else
        {
            var migration = migrationsCompleted > 1 ? "migrations" : "migration";
            this.Log.Info(
                $"{Environment.NewLine}Ran {migrationsCompleted} {migration} in {(DateTime.UtcNow - startAt).TotalSeconds:N3}s");
        }

        return new AppTaskResult(migrationsRun);
    }

    /// <summary>
    /// Logs the migrations found.
    /// </summary>
    /// <param name="remainingMigrations">The remaining migrations.</param>
    private void LogMigrationsFound(List<Type> remainingMigrations)
    {
        var sb = StringBuilderCache.Allocate()
            .AppendLine("Migrations Found:");
        remainingMigrations.Each(x => sb.AppendLine((string?)$" - {x.Name}"));
        this.Log.Info(StringBuilderCache.ReturnAndFree(sb));
    }

    /// <summary>
    /// Gets all migration types.
    /// </summary>
    /// <param name="migrationAssemblies">The migration assemblies.</param>
    /// <returns>System.Collections.Generic.List&lt;System.Type&gt;.</returns>
    public static List<Type> GetAllMigrationTypes(params Assembly[] migrationAssemblies)
    {
        var remainingMigrations = migrationAssemblies
            .SelectMany(x => x.GetTypes().Where(type => type.IsInstanceOf(typeof(MigrationBase)) && !type.IsAbstract))
            .OrderBy(x => x.Name)
            .ToList();
        return remainingMigrations;
    }

    /// <summary>
    /// Initializes the specified database.
    /// </summary>
    /// <param name="db">The database.</param>
    public static void Init(IDbConnection db)
    {
        db.CreateTableIfNotExists<Migration>();
    }

    /// <summary>
    /// Recreates the specified database.
    /// </summary>
    /// <param name="db">The database.</param>
    public static void Recreate(IDbConnection db)
    {
        db.DropAndCreateTable<Migration>();
    }

    /// <summary>
    /// Clears the specified database.
    /// </summary>
    /// <param name="db">The database.</param>
    public static void Clear(IDbConnection db)
    {
        if (db.TableExists<Migration>())
        {
            db.DeleteAll<Migration>();
        }
        else
        {
            Init(db);
        }
    }

    /// <summary>
    /// Gets the next migration revert to run.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="migrationTypes">The migration types.</param>
    /// <returns>System.Type?.</returns>
    /// <exception cref="InfoException">$"Migration '{lastRun.Name}' is still in progress, timeout in {(Timeout - elapsedTime).TotalSeconds:N3}s.</exception>
    /// <exception cref="InfoException">$"Could not find Migration '{lastRun.Name}' to revert, aborting.</exception>
    private Type? GetNextMigrationRevertToRun(IDbConnection db, List<Type> migrationTypes)
    {
        var q = db.From<Migration>()
            .OrderByDescending(x => x.Name).Limit(1);
        Migration? lastRun;
        while (true)
        {
            lastRun = db.Single(q);
            if (lastRun == null)
            {
                return null;
            }

            var elapsedTime = DateTime.UtcNow - lastRun.CreatedDate;
            if (lastRun.CompletedDate == null)
            {
                if (elapsedTime < this.Timeout)
                {
                    throw new InfoException(
                        $"Migration '{lastRun.Name}' is still in progress, timeout in {(this.Timeout - elapsedTime).TotalSeconds:N3}s.");
                }

                this.Log.Info(
                    $"Migration '{lastRun.Name}' failed to complete {elapsedTime.TotalSeconds:N3}s ago, ignoring...");
                db.DeleteById<Migration>(lastRun.Id);
                continue;
            }

            var nextMigration = migrationTypes.FirstOrDefault(x => x.Name == lastRun.Name) ??
                                throw new InfoException(
                                    $"Could not find Migration '{lastRun.Name}' to revert, aborting.");

            return nextMigration;
        }
    }

    /// <summary>
    /// Reruns the specified migration name.
    /// </summary>
    /// <param name="migrationName">Name of the migration.</param>
    /// <returns>ServiceStack.AppTaskResult.</returns>
    public AppTaskResult Rerun(string? migrationName)
    {
        Revert(migrationName, throwIfError: true);
        if (migrationName is Last or All)
        {
            return Run(throwIfError: true);
        }

        var migrationType = MigrationTypes.FirstOrDefault(x => x.Name == migrationName);
        if (migrationType == null)
        {
            throw new InfoException($"Could not find Migration '{migrationName}' to rerun, aborting.");
        }

        var migration = Run(DbFactory, migrationType, x => x.Up());
        return new AppTaskResult([migration]);
    }

    /// <summary>
    /// Reverts the specified migration name.
    /// </summary>
    /// <param name="migrationName">Name of the migration.</param>
    /// <returns>ServiceStack.AppTaskResult.</returns>
    public AppTaskResult Revert(string? migrationName)
    {
        return this.Revert(migrationName, throwIfError: true);
    }

    /// <summary>
    /// Reverts the specified migration name.
    /// </summary>
    /// <param name="migrationName">Name of the migration.</param>
    /// <param name="throwIfError">The throw if error.</param>
    /// <returns>ServiceStack.AppTaskResult.</returns>
    public AppTaskResult Revert(string? migrationName, bool throwIfError)
    {
        using var db = this.DbFactory.Open();
        Init(db);

        var allMigrationTypes = this.MigrationTypes.ToList();
        allMigrationTypes.Reverse();

        this.LogMigrationsFound(allMigrationTypes);

        var startAt = DateTime.UtcNow;
        var migrationsRun = new List<IAppTask>();

        this.Log.Info($"Reverting {migrationName}...");

        migrationName = migrationName switch {
            All => allMigrationTypes.LastOrDefault()?.Name,
            Last => allMigrationTypes.FirstOrDefault()?.Name,
            _ => migrationName
        };

        if (migrationName != null)
        {
            while (true)
            {
                Type? nextRun;
                try
                {
                    nextRun = this.GetNextMigrationRevertToRun(db, allMigrationTypes);
                    if (nextRun == null)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    this.Log.Error(e.Message);
                    if (throwIfError)
                    {
                        throw;
                    }

                    return new AppTaskResult(migrationsRun) { Error = e };
                }

                var migrationStartAt = DateTime.UtcNow;

                var namedConnections = nextRun.AllAttributes<NamedConnectionAttribute>().Select(x => x.Name ?? null)
                    .ToArray();
                if (namedConnections.Length == 0)
                {
                    namedConnections = [null];
                }

                foreach (var namedConnection in namedConnections)
                {
                    var descFmt = AppTasks.GetDescFmt(nextRun);
                    var namedDesc = namedConnection == null ? "" : $" ({namedConnection})";
                    this.Log.Info($"Reverting {nextRun.Name}{descFmt}{namedDesc}...");

                    var instance = Run(this.DbFactory, nextRun, x => x.Down(), namedConnection);
                    migrationsRun.Add(instance);
                    this.Log.Info(instance.Log);

                    if (instance.Error == null)
                    {
                        this.Log.Info(
                            $"Completed revert of {nextRun.Name}{descFmt} in {(DateTime.UtcNow - migrationStartAt).TotalSeconds:N3}s" +
                            Environment.NewLine);

                        // Remove completed migration revert from DB
                        db.Delete<Migration>(x => x.Name == nextRun.Name && x.NamedConnection == namedConnection);
                    }
                    else
                    {
                        this.Log.Error(instance.Error.Message, instance.Error);
                        if (throwIfError)
                        {
                            throw instance.Error;
                        }

                        return new AppTaskResult(migrationsRun);
                    }
                }

                if (migrationName == nextRun.Name)
                {
                    break;
                }
            }
        }

        var migrationsCompleted = migrationsRun.Count(x => x.Error == null);
        if (migrationsCompleted == 0)
        {
            this.Log.Info("No migrations were reverted.");
        }
        else
        {
            var migration = migrationsCompleted > 1 ? "migrations" : "migration";
            this.Log.Info(
                $"{Environment.NewLine}Reverted {migrationsCompleted} {migration} in {(DateTime.UtcNow - startAt).TotalSeconds:N3}s");
        }

        return new AppTaskResult(migrationsRun);
    }

    /// <summary>
    /// Downs the specified database factory.
    /// </summary>
    /// <param name="dbFactory">The database factory.</param>
    /// <param name="migrationType">Type of the migration.</param>
    /// <returns>ServiceStack.AppTaskResult.</returns>
    public static AppTaskResult Down(IDbConnectionFactory dbFactory, Type migrationType)
    {
        return Down(dbFactory,
            [migrationType]);
    }

    /// <summary>
    /// Downs the specified database factory.
    /// </summary>
    /// <param name="dbFactory">The database factory.</param>
    /// <param name="migrationTypes">The migration types.</param>
    /// <returns>ServiceStack.AppTaskResult.</returns>
    public static AppTaskResult Down(IDbConnectionFactory dbFactory, Type[] migrationTypes)
    {
        return RunAll(dbFactory, migrationTypes, x => x.Down());
    }

    /// <summary>
    /// Ups the specified database factory.
    /// </summary>
    /// <param name="dbFactory">The database factory.</param>
    /// <param name="migrationType">Type of the migration.</param>
    /// <returns>ServiceStack.AppTaskResult.</returns>
    public static AppTaskResult Up(IDbConnectionFactory dbFactory, Type migrationType)
    {
        return Up(dbFactory, [migrationType]);
    }

    /// <summary>
    /// Ups the specified database factory.
    /// </summary>
    /// <param name="dbFactory">The database factory.</param>
    /// <param name="migrationTypes">The migration types.</param>
    /// <returns>ServiceStack.AppTaskResult.</returns>
    public static AppTaskResult Up(IDbConnectionFactory dbFactory, Type[] migrationTypes)
    {
        return RunAll(dbFactory, migrationTypes, x => x.Up());
    }

    /// <summary>
    /// Runs the specified database factory.
    /// </summary>
    /// <param name="dbFactory">The database factory.</param>
    /// <param name="nextRun">The next run.</param>
    /// <param name="migrateAction">The migrate action.</param>
    /// <param name="namedConnection">The named connection.</param>
    /// <returns>ServiceStack.OrmLite.MigrationBase.</returns>
    public static MigrationBase Run(IDbConnectionFactory dbFactory, Type nextRun, Action<MigrationBase> migrateAction,
        string? namedConnection = null)
    {
        var holdFilter = OrmLiteConfig.BeforeExecFilter;
        var instance = nextRun.CreateInstance<MigrationBase>();
        OrmLiteConfig.BeforeExecFilter = dbCmd => instance.MigrationLog.AppendLine(dbCmd.GetDebugString());

        IDbConnection? useDb = null;
        IDbTransaction? trans = null;

        try
        {
            useDb = namedConnection == null
                ? dbFactory.OpenDbConnection()
                : dbFactory.OpenDbConnection(namedConnection);

            instance.DbFactory = dbFactory;
            instance.Db = useDb;
            trans = useDb.OpenTransaction();
            instance.AfterOpen();

            instance.Transaction = trans;
            instance.StartedAt = DateTime.UtcNow;

            // Run Migration
            migrateAction(instance);

            instance.CompletedDate = DateTime.UtcNow;
            instance.Log = instance.MigrationLog.ToString();

            instance.BeforeCommit();
            trans.Commit();
            trans.Dispose();
            trans = null;
        }
        catch (Exception e)
        {
            instance.CompletedDate = DateTime.UtcNow;
            instance.Error = e;
            instance.Log = instance.MigrationLog.ToString();
            try
            {
                instance.BeforeRollback();
                trans?.Rollback();
                trans?.Dispose();
            }
            catch (Exception exRollback)
            {
                instance.Log += $"{Environment.NewLine}{exRollback.Message}";
            }
        }
        finally
        {
            instance.Db = null;
            instance.Transaction = null;
            OrmLiteConfig.BeforeExecFilter = holdFilter;
            try
            {
                useDb?.Dispose();
            }
            catch (Exception exRollback)
            {
                instance.Log += $"{Environment.NewLine}{exRollback.Message}";
            }
        }

        return instance;
    }

    /// <summary>
    /// Runs all.
    /// </summary>
    /// <param name="dbFactory">The database factory.</param>
    /// <param name="migrationTypes">The migration types.</param>
    /// <param name="migrateAction">The migrate action.</param>
    /// <returns>ServiceStack.AppTaskResult.</returns>
    public static AppTaskResult RunAll(IDbConnectionFactory dbFactory, IEnumerable<Type> migrationTypes,
        Action<MigrationBase> migrateAction)
    {
        var migrationsRun = new List<IAppTask>();

        foreach (var instance in migrationTypes.Select(nextRun => Run(dbFactory, nextRun, migrateAction)))
        {
            migrationsRun.Add(instance);
            if (instance.Error != null)
            {
                break;
            }
        }

        return new AppTaskResult(migrationsRun);
    }
}