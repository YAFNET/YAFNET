/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Data;

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using YAF.Types.Interfaces.Data;

/// <summary>
///     The DB access base.
/// </summary>
public abstract class DbAccessBase : IDbAccess
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbAccessBase"/> class.
    /// </summary>
    /// <param name="dbProviderFactory">
    /// The db provider factory.
    /// </param>
    /// <param name="information">
    /// The information.
    /// </param>
    protected DbAccessBase(
        Func<string, DbProviderFactory> dbProviderFactory, IDbInformation information)
    {
        this.Information = information;
        this.DbProviderFactory = dbProviderFactory(information.ProviderName);
    }

    /// <summary>
    ///     Gets or sets ConnectionString.
    /// </summary>
    public virtual IDbInformation Information { get; protected set; }

    /// <summary>
    ///     Gets or sets DbProviderFactory.
    /// </summary>
    public virtual DbProviderFactory DbProviderFactory { get; protected set; }

    /// <summary>
    /// The execute.
    /// </summary>
    /// <param name="execFunc">
    /// The exec func.
    /// </param>
    /// <param name="cmd">
    /// The cmd.
    /// </param>
    /// <param name="dbTransaction">
    /// The db transaction.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// The <see cref="T"/>.
    /// </returns>
    public virtual T Execute<T>(Func<IDbCommand, T> execFunc, IDbCommand cmd = null, IDbTransaction dbTransaction = null)
    {
        var command = cmd ?? this.GetCommand(string.Empty, CommandType.Text);

        T result;

        if (dbTransaction == null)
        {
            if (command.Connection is { State: ConnectionState.Open })
            {
                result = execFunc(command);
            }
            else
            {
                using var connection = this.CreateConnectionOpen();
                // get an open connection
                command.Connection = connection;

                result = execFunc(command);

                connection.Close();
            }
        }
        else
        {
            command.Populate(dbTransaction);

            result = execFunc(command);
        }

        return result;
    }

    /// <summary>
    /// The execute.
    /// </summary>
    /// <param name="execFunc">
    /// The exec func.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// The <see cref="T"/>.
    /// </returns>
    public async virtual Task<T> ExecuteAsync<T>(Func<IDbConnection, Task<T>> execFunc)
    {
        using var connection = await this.CreateConnectionOpenAsync();

        var result = await execFunc(connection);

        connection.Close();

        return result;
    }

    /// <summary>
    /// The get command.
    /// </summary>
    /// <param name="sql">
    /// The sql.
    /// </param>
    /// <param name="commandType"></param>
    /// <param name="parameters">
    /// The parameters.
    /// </param>
    /// <returns>
    /// The <see cref="DbCommand"/> .
    /// </returns>
    public virtual IDbCommand GetCommand(
        string sql, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters = null)
    {
        var cmd = this.DbProviderFactory.CreateCommand();
        parameters = parameters.IfNullEmpty();

        cmd!.CommandTimeout = Config.SqlCommandTimeout;
        cmd.CommandType = commandType;

        cmd.CommandText = sql;

        // map parameters for this command...
        this.MapParameters(cmd, parameters);

        return cmd.ReplaceCommandText();
    }

    /// <summary>
    /// The map parameters.
    /// </summary>
    /// <param name="cmd">
    /// The cmd.
    /// </param>
    /// <param name="parameters">
    /// The parameters.
    /// </param>
    protected virtual void MapParameters(IDbCommand cmd, IEnumerable<KeyValuePair<string, object>> parameters)
    {
        var keyValuePairs = parameters.ToList();

        ArgumentNullException.ThrowIfNull(cmd);
        ArgumentNullException.ThrowIfNull(keyValuePairs);

        // add all/any parameters...
        keyValuePairs.ForEach(cmd.AddParam);
    }
}