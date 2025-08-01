// ***********************************************************************
// <copyright file="SingleWriterDbConnection.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable
using System;
using System.Data;
using System.Data.Common;
using ServiceStack.Data;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

public class SingleWriterDbConnection : DbConnection, IHasWriteLock
{
    private DbConnection? db;
    public OrmLiteConnectionFactory? Factory { get; }
    public object WriteLock { get; }

    public DbConnection Db => this.db ??= (DbConnection)this.ConnectionString.ToDbConnection(this.Factory!.DialectProvider);

    public SingleWriterDbConnection(DbConnection db, object writeLock)
    {
        this.db = db;
        this.WriteLock = writeLock;
        this.connectionString = db.ConnectionString;
    }

    public SingleWriterDbConnection(OrmLiteConnectionFactory factory, object writeLock)
    {
        this.Factory = factory;
        this.WriteLock = writeLock;
        this.connectionString = factory.ConnectionString;
    }

    internal DbTransaction? Transaction;

    override protected DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    {
        this.Transaction = this.Db.BeginTransaction(isolationLevel);
        return new SingleWriterTransaction(this, this.Transaction, isolationLevel);
    }

    public override void Close()
    {
        this.Db.Close();
    }

    public override void ChangeDatabase(string databaseName)
    {
        this.Db.ChangeDatabase(databaseName);
    }

    public override void Open()
    {
        var dbConn = this.Db;
        if (dbConn.State == ConnectionState.Broken)
        {
            dbConn.Close();
        }

        if (dbConn.State == ConnectionState.Closed)
        {
            var id = Diagnostics.OrmLite.WriteConnectionOpenBefore(dbConn);
            Exception? e = null;
            try
            {
                dbConn.Open();
                if (this.Factory != null)
                {
                    //so the internal connection is wrapped for example by miniprofiler
                    if (this.Factory.ConnectionFilter != null)
                    {
                        dbConn = (DbConnection)this.Factory.ConnectionFilter(dbConn);
                    }

                    this.Factory.DialectProvider.InitConnection(this);
                }
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

    private string connectionString;
    public override string ConnectionString {
        get => this.connectionString;
        set => this.connectionString = value;
    }

    public override string Database => this.Db.Database;
    public override ConnectionState State => this.Db.State;
    public override string? DataSource => this.Db.DataSource;
    public override string? ServerVersion => this.Db.ServerVersion;

    override protected DbCommand CreateDbCommand()
    {
        var dbCmd = this.Db.CreateCommand();
        return new SingleWriterDbCommand(this, dbCmd, this.WriteLock);
    }

    override protected void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.db?.Close();
            this.db?.Dispose();
            this.db = null;
        }
        base.Dispose(disposing);
    }
}

public class SingleWriterDbCommand(SingleWriterDbConnection db, DbCommand cmd, object writeLock) : DbCommand
{
    private readonly SingleWriterDbConnection Db = db;

    public override void Prepare()
    {
        cmd.Prepare();
    }

    public override string CommandText {
        get => cmd.CommandText;
        set => cmd.CommandText = value;
    }

    public override int CommandTimeout {
        get => cmd.CommandTimeout;
        set => cmd.CommandTimeout = value;
    }

    public override CommandType CommandType {
        get => cmd.CommandType;
        set => cmd.CommandType = value;
    }

    public override UpdateRowSource UpdatedRowSource {
        get => cmd.UpdatedRowSource;
        set => cmd.UpdatedRowSource = value;
    }

    override protected DbConnection? DbConnection {
        get => cmd.Connection;
        set => cmd.Connection = value;
    }

    override protected DbParameterCollection DbParameterCollection => cmd.Parameters;

    override protected DbTransaction? DbTransaction {
        get => this.Db.Transaction;
        set => this.Db.Transaction = value;
    }

    public override bool DesignTimeVisible {
        get => cmd.DesignTimeVisible;
        set => cmd.DesignTimeVisible = value;
    }

    public override void Cancel()
    {
        cmd.Cancel();
    }

    override protected DbParameter CreateDbParameter()
    {
        return cmd.CreateParameter();
    }

    override protected DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
    {
        return cmd.ExecuteReader(behavior);
    }

    public override int ExecuteNonQuery()
    {
        lock (writeLock)
        {
            return cmd.ExecuteNonQuery();
        }
    }

    public override object? ExecuteScalar()
    {
        return cmd.ExecuteScalar();
    }

    override protected void Dispose(bool disposing)
    {
        if (disposing)
        {
            cmd?.Dispose();
        }
        base.Dispose(disposing);
    }
}

public static class SingleWriterExtensions
{
    public static DbConnection WithWriteLock(this IDbConnection db, object writeLock)
    {
        return db switch
        {
            SingleWriterDbConnection writeLockConn => writeLockConn,
            OrmLiteConnection ormConn =>
                new SingleWriterDbConnection((DbConnection)ormConn.ToDbConnection(), writeLock),
            _ => new SingleWriterDbConnection((DbConnection)db, writeLock)
        };
    }

    /// <summary>
    /// Open a DB connection with a SingleWriter Lock 
    /// </summary>
    public static DbConnection OpenSingleWriterDb(this IDbConnectionFactory dbFactory, string? namedConnection = null)
    {
        var dbConn = namedConnection != null
            ? dbFactory.OpenDbConnection(namedConnection)
            : dbFactory.OpenDbConnection();
        var writeLock = dbConn.GetWriteLock() ?? Locks.GetDbLock(namedConnection);
        return dbConn.WithWriteLock(writeLock);
    }

    /// <summary>
    /// Create a DB connection with a SingleWriter Lock 
    /// </summary>
    public static DbConnection CreateSingleWriterDb(this IDbConnectionFactory dbFactory, string? namedConnection = null)
    {
        return ((OrmLiteConnectionFactory)dbFactory).CreateDbWithWriteLock(namedConnection);
    }

    public static object GetWriteLock(this IDbConnection dbConnection)
    {
        return dbConnection switch
        {
            OrmLiteConnection dbConn => dbConn.WriteLock ?? dbConnection,
            IHasWriteLock hasWriteLock => hasWriteLock.WriteLock,
            _ => dbConnection
        };
    }
}

public class SingleWriterTransaction(SingleWriterDbConnection dbConnection, DbTransaction transaction, IsolationLevel isolationLevel) : DbTransaction
{
    override protected DbConnection DbConnection { get; } = dbConnection;
    public override IsolationLevel IsolationLevel { get; } = isolationLevel;
    public readonly DbTransaction Transaction = transaction;

    public override void Commit()
    {
        this.Transaction.Commit();
    }

    public override void Rollback()
    {
        this.Transaction.Rollback();
    }

    override protected void Dispose(bool disposing)
    {
        dbConnection.Transaction = null;
        base.Dispose(disposing);
    }
}