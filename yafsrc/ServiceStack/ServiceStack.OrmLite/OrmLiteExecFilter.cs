// ***********************************************************************
// <copyright file="OrmLiteExecFilter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.OrmLite;

using ServiceStack.Text;

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

/// <summary>
/// Interface IOrmLiteExecFilter
/// </summary>
public interface IOrmLiteExecFilter
{
    /// <summary>
    /// SQLs the expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    SqlExpression<T> SqlExpression<T>(IDbConnection dbConn);
    /// <summary>
    /// Creates the command.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <returns>IDbCommand.</returns>
    IDbCommand CreateCommand(IDbConnection dbConn);
    /// <summary>
    /// Disposes the command.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="dbConn">The database connection.</param>
    void DisposeCommand(IDbCommand dbCmd, IDbConnection dbConn);
    /// <summary>
    /// Executes the specified database connection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>T.</returns>
    T Exec<T>(IDbConnection dbConn, Func<IDbCommand, T> filter);
    /// <summary>
    /// Executes the specified database connection.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>IDbCommand.</returns>
    IDbCommand Exec(IDbConnection dbConn, Func<IDbCommand, IDbCommand> filter);
    /// <summary>
    /// Executes the specified database connection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>Task&lt;T&gt;.</returns>
    Task<T> Exec<T>(IDbConnection dbConn, Func<IDbCommand, Task<T>> filter);
    /// <summary>
    /// Executes the specified database connection.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>Task&lt;IDbCommand&gt;.</returns>
    Task<IDbCommand> Exec(IDbConnection dbConn, Func<IDbCommand, Task<IDbCommand>> filter);
    /// <summary>
    /// Executes the specified database connection.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="filter">The filter.</param>
    void Exec(IDbConnection dbConn, Action<IDbCommand> filter);
    /// <summary>
    /// Executes the specified database connection.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>Task.</returns>
    Task Exec(IDbConnection dbConn, Func<IDbCommand, Task> filter);
    /// <summary>
    /// Executes the lazy.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>IEnumerable&lt;T&gt;.</returns>
    IEnumerable<T> ExecLazy<T>(IDbConnection dbConn, Func<IDbCommand, IEnumerable<T>> filter);
}

/// <summary>
/// Class OrmLiteExecFilter.
/// Implements the <see cref="ServiceStack.OrmLite.IOrmLiteExecFilter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.IOrmLiteExecFilter" />
public class OrmLiteExecFilter : IOrmLiteExecFilter
{
    /// <summary>
    /// SQLs the expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    public virtual SqlExpression<T> SqlExpression<T>(IDbConnection dbConn)
    {
        return dbConn.GetDialectProvider().SqlExpression<T>();
    }

    /// <summary>
    /// Creates the command.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <returns>IDbCommand.</returns>
    public virtual IDbCommand CreateCommand(IDbConnection dbConn)
    {
        var ormLiteConn = dbConn as OrmLiteConnection;

        var dbCmd = dbConn.CreateCommand();

        dbCmd.Transaction = ormLiteConn != null
                                ? ormLiteConn.Transaction
                                : OrmLiteContext.TSTransaction;

        dbCmd.CommandTimeout = ormLiteConn != null
                                   ? ormLiteConn.CommandTimeout ?? OrmLiteConfig.CommandTimeout
                                   : OrmLiteConfig.CommandTimeout;

        ormLiteConn.SetLastCommandText(null);

        return new OrmLiteCommand(ormLiteConn, dbCmd);
    }

    /// <summary>
    /// Disposes the command.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="dbConn">The database connection.</param>
    public virtual void DisposeCommand(IDbCommand dbCmd, IDbConnection dbConn)
    {
        if (dbCmd == null)
        {
            return;
        }

        OrmLiteConfig.AfterExecFilter?.Invoke(dbCmd);

        dbConn.SetLastCommandText(dbCmd.CommandText);

        dbCmd.Dispose();
    }

    /// <summary>
    /// Executes the specified database connection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>T.</returns>
    public virtual T Exec<T>(IDbConnection dbConn, Func<IDbCommand, T> filter)
    {
        var dbCmd = this.CreateCommand(dbConn);

        try
        {
            var ret = filter(dbCmd);

            return ret;
        }
        catch (Exception ex)
        {
            OrmLiteConfig.ExceptionFilter?.Invoke(dbCmd, ex);
            throw;
        }
        finally
        {
            this.DisposeCommand(dbCmd, dbConn);
        }
    }

    /// <summary>
    /// Executes the specified database connection.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>IDbCommand.</returns>
    public virtual IDbCommand Exec(IDbConnection dbConn, Func<IDbCommand, IDbCommand> filter)
    {
        var dbCmd = this.CreateCommand(dbConn);
        var ret = filter(dbCmd);
        if (dbCmd != null)
        {
            dbConn.SetLastCommandText(dbCmd.CommandText);
        }

        return ret;
    }

    /// <summary>
    /// Executes the specified database connection.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="filter">The filter.</param>
    public virtual void Exec(IDbConnection dbConn, Action<IDbCommand> filter)
    {
        var dbCmd = this.CreateCommand(dbConn);
        try
        {
            filter(dbCmd);
        }
        catch (Exception ex)
        {
            OrmLiteConfig.ExceptionFilter?.Invoke(dbCmd, ex);
            throw;
        }
        finally
        {
            this.DisposeCommand(dbCmd, dbConn);
        }
    }

    /// <summary>
    /// Executes the specified database connection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>T.</returns>
    public virtual async Task<T> Exec<T>(IDbConnection dbConn, Func<IDbCommand, Task<T>> filter)
    {
        var dbCmd = this.CreateCommand(dbConn);

        try
        {
            return await filter(dbCmd);
        }
        catch (Exception ex)
        {
            var useEx = ex.UnwrapIfSingleException();
            OrmLiteConfig.ExceptionFilter?.Invoke(dbCmd, useEx);
            throw useEx;
        }
        finally
        {
            this.DisposeCommand(dbCmd, dbConn);
        }
    }

    /// <summary>
    /// Executes the specified database connection.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>IDbCommand.</returns>
    public virtual async Task<IDbCommand> Exec(IDbConnection dbConn, Func<IDbCommand, Task<IDbCommand>> filter)
    {
        var dbCmd = this.CreateCommand(dbConn);
        return await filter(dbCmd);
    }

    /// <summary>
    /// Executes the specified database connection.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="filter">The filter.</param>
    public virtual async Task Exec(IDbConnection dbConn, Func<IDbCommand, Task> filter)
    {
        var dbCmd = this.CreateCommand(dbConn);

        try
        {
            await filter(dbCmd);
        }
        catch (Exception ex)
        {
            var useEx = ex.UnwrapIfSingleException();
            OrmLiteConfig.ExceptionFilter?.Invoke(dbCmd, useEx);
            throw useEx;
        }
        finally
        {
            this.DisposeCommand(dbCmd, dbConn);
        }
    }

    /// <summary>
    /// Executes the lazy.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbConn">The database connection.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>IEnumerable&lt;T&gt;.</returns>
    public virtual IEnumerable<T> ExecLazy<T>(IDbConnection dbConn, Func<IDbCommand, IEnumerable<T>> filter)
    {
        var dbCmd = this.CreateCommand(dbConn);
        try
        {
            var results = filter(dbCmd);

            foreach (var item in results)
            {
                yield return item;
            }
        }
        finally
        {
            this.DisposeCommand(dbCmd, dbConn);
        }
    }
}