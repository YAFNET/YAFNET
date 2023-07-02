// ***********************************************************************
// <copyright file="PostgreSqlExecFilter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Data;
using Npgsql;

namespace ServiceStack.OrmLite.PostgreSQL;

/// <summary>
/// Class PostgreSqlExecFilter.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteExecFilter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteExecFilter" />
public class PostgreSqlExecFilter : OrmLiteExecFilter 
{
    /// <summary>
    /// Gets or sets the on command.
    /// </summary>
    /// <value>The on command.</value>
    public Action<NpgsqlCommand> OnCommand { get; set; }

    /// <summary>
    /// Creates the command.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    /// <returns>IDbCommand.</returns>
    public override IDbCommand CreateCommand(IDbConnection dbConn)
    {
        var cmd = base.CreateCommand(dbConn);
        if (OnCommand != null)
        {
            if (cmd.ToDbCommand() is NpgsqlCommand psqlCmd)
            {
                OnCommand?.Invoke(psqlCmd);
            }
        }
        return cmd;
    }
}