// ***********************************************************************
// <copyright file="PostgreSqlExecFilter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Data;
using Npgsql;

namespace ServiceStack.OrmLite.PostgreSQL
{
    public class PostgreSqlExecFilter : OrmLiteExecFilter 
    {
        public Action<NpgsqlCommand> OnCommand { get; set; }

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
}
