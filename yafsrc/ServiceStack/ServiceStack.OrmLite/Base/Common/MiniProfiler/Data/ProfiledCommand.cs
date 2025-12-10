// ***********************************************************************
// <copyright file="ProfiledCommand.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data.Common;
using System.Data;
using ServiceStack.Data;

#pragma warning disable 1591 // xml doc comments warnings

namespace ServiceStack.MiniProfiler.Data;

/// <summary>
/// Class ProfiledCommand.
/// Implements the <see cref="System.Data.Common.DbCommand" />
/// Implements the <see cref="ServiceStack.Data.IHasDbCommand" />
/// </summary>
/// <seealso cref="System.Data.Common.DbCommand" />
/// <seealso cref="ServiceStack.Data.IHasDbCommand" />
public class ProfiledCommand : DbCommand, IHasDbCommand
{
    /// <summary>
    /// The command
    /// </summary>
    private DbCommand cmd;
    /// <summary>
    /// The connection
    /// </summary>
    private DbConnection conn;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfiledCommand" /> class.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="conn">The connection.</param>
    /// <param name="profiler">The profiler.</param>
    /// <exception cref="System.ArgumentNullException">cmd</exception>
    public ProfiledCommand(DbCommand cmd, DbConnection conn, IDbProfiler profiler)
    {
        this.cmd = cmd ?? throw new ArgumentNullException(nameof(cmd));
        this.conn = conn;

        if (profiler != null)
        {
            this.DbProfiler = profiler;
        }
    }

    /// <summary>
    /// Gets or sets the text command to run against the data source.
    /// </summary>
    /// <value>The command text.</value>
    public override string CommandText
    {
        get => this.cmd.CommandText;
        set => this.cmd.CommandText = value;
    }

    /// <summary>
    /// Gets or sets the wait time before terminating the attempt to execute a command and generating an error.
    /// </summary>
    /// <value>The command timeout.</value>
    public override int CommandTimeout
    {
        get => this.cmd.CommandTimeout;
        set => this.cmd.CommandTimeout = value;
    }

    /// <summary>
    /// Indicates or specifies how the <see cref="P:System.Data.Common.DbCommand.CommandText" /> property is interpreted.
    /// </summary>
    /// <value>The type of the command.</value>
    public override CommandType CommandType
    {
        get => this.cmd.CommandType;
        set => this.cmd.CommandType = value;
    }

    /// <summary>
    /// Gets the database command.
    /// </summary>
    /// <value>The database command.</value>
    public DbCommand DbCommand
    {
        get => this.cmd;
        protected set => this.cmd = value;
    }

    /// <summary>
    /// Gets the database command.
    /// </summary>
    /// <value>The database command.</value>
    IDbCommand IHasDbCommand.DbCommand => this.DbCommand;

    /// <summary>
    /// Gets or sets the <see cref="T:System.Data.Common.DbConnection" /> used by this <see cref="T:System.Data.Common.DbCommand" />.
    /// </summary>
    /// <value>The database connection.</value>
    override protected DbConnection DbConnection
    {
        get => this.conn;
        set
        {
            this.conn = value;
            this.cmd.Connection = value is not ProfiledConnection awesomeConn ? value : awesomeConn.WrappedConnection;
        }
    }

    /// <summary>
    /// Gets the collection of <see cref="T:System.Data.Common.DbParameter" /> objects.
    /// </summary>
    /// <value>The database parameter collection.</value>
    override protected DbParameterCollection DbParameterCollection => this.cmd.Parameters;

    /// <summary>
    /// Gets or sets the <see cref="P:System.Data.Common.DbCommand.DbTransaction" /> within which this <see cref="T:System.Data.Common.DbCommand" /> object executes.
    /// </summary>
    /// <value>The database transaction.</value>
    override protected DbTransaction DbTransaction {
        get;
        set {
            field = value;
            this.cmd.Transaction =
                value is not ProfiledDbTransaction { DbTransaction: System.Data.Common.DbTransaction } awesomeTran
                    ? value
                    : (DbTransaction)awesomeTran.DbTransaction;
        }
    }

    /// <summary>
    /// Gets or sets the database profiler.
    /// </summary>
    /// <value>The database profiler.</value>
    protected IDbProfiler DbProfiler { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the command object should be visible in a customized interface control.
    /// </summary>
    /// <value><c>true</c> if [design time visible]; otherwise, <c>false</c>.</value>
    public override bool DesignTimeVisible
    {
        get => this.cmd.DesignTimeVisible;
        set => this.cmd.DesignTimeVisible = value;
    }

    /// <summary>
    /// Gets or sets how command results are applied to the <see cref="T:System.Data.DataRow" /> when used by the Update method of a <see cref="T:System.Data.Common.DbDataAdapter" />.
    /// </summary>
    /// <value>The updated row source.</value>
    public override UpdateRowSource UpdatedRowSource
    {
        get => this.cmd.UpdatedRowSource;
        set => this.cmd.UpdatedRowSource = value;
    }


    /// <summary>
    /// Executes the command text against the connection.
    /// </summary>
    /// <param name="behavior">An instance of <see cref="T:System.Data.CommandBehavior" />.</param>
    /// <returns>A task representing the operation.</returns>
    override protected DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
    {
        if (this.DbProfiler is not {IsActive: true})
        {
            return this.cmd.ExecuteReader(behavior);
        }

        DbDataReader result = null;
        this.DbProfiler.ExecuteStart(this, ExecuteType.Reader);
        try
        {
            result = this.cmd.ExecuteReader(behavior);
            result = new ProfiledDbDataReader(result, this.conn, this.DbProfiler);
        }
        catch (Exception e)
        {
            this.DbProfiler.OnError(this, ExecuteType.Reader, e);
            throw;
        }
        finally
        {
            this.DbProfiler.ExecuteFinish(this, ExecuteType.Reader, result);
        }
        return result;
    }

    /// <summary>
    /// Executes a SQL statement against a connection object.
    /// </summary>
    /// <returns>The number of rows affected.</returns>
    public override int ExecuteNonQuery()
    {
        if (this.DbProfiler is not {IsActive: true})
        {
            return this.cmd.ExecuteNonQuery();
        }

        int result;

        this.DbProfiler.ExecuteStart(this, ExecuteType.NonQuery);
        try
        {
            result = this.cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            this.DbProfiler.OnError(this, ExecuteType.NonQuery, e);
            throw;
        }
        finally
        {
            this.DbProfiler.ExecuteFinish(this, ExecuteType.NonQuery, null);
        }
        return result;
    }

    /// <summary>
    /// Executes the query and returns the first column of the first row in the result set returned by the query. All other columns and rows are ignored.
    /// </summary>
    /// <returns>The first column of the first row in the result set.</returns>
    public override object ExecuteScalar()
    {
        if (this.DbProfiler is not {IsActive: true})
        {
            return this.cmd.ExecuteScalar();
        }

        object result;
        this.DbProfiler.ExecuteStart(this, ExecuteType.Scalar);
        try
        {
            result = this.cmd.ExecuteScalar();
        }
        catch (Exception e)
        {
            this.DbProfiler.OnError(this, ExecuteType.Scalar, e);
            throw;
        }
        finally
        {
            this.DbProfiler.ExecuteFinish(this, ExecuteType.Scalar, null);
        }
        return result;
    }

    /// <summary>
    /// Attempts to cancels the execution of a <see cref="T:System.Data.Common.DbCommand" />.
    /// </summary>
    public override void Cancel()
    {
        this.cmd.Cancel();
    }

    /// <summary>
    /// Creates a prepared (or compiled) version of the command on the data source.
    /// </summary>
    public override void Prepare()
    {
        this.cmd.Prepare();
    }

    /// <summary>
    /// Creates a new instance of a <see cref="T:System.Data.Common.DbParameter" /> object.
    /// </summary>
    /// <returns>A <see cref="T:System.Data.Common.DbParameter" /> object.</returns>
    override protected DbParameter CreateDbParameter()
    {
        return this.cmd.CreateParameter();
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component" /> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
    override protected void Dispose(bool disposing)
    {
        if (disposing && this.cmd != null)
        {
            this.cmd.Dispose();
        }
        this.cmd = null;
        base.Dispose(disposing);
    }
}

#pragma warning restore 1591 // xml doc comments warnings