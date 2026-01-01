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

namespace YAF.Types.Interfaces.Data;

using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

/// <summary>
///     DBAccess Interface
/// </summary>
public interface IDbAccess
{
    /// <summary>
    ///     Gets the database information
    /// </summary>
    IDbInformation Information { get; }

    /// <summary>
    ///     Gets the current Database provider factory
    /// </summary>
    /// <returns> </returns>
    DbProviderFactory DbProviderFactory { get; }

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
    T Execute<T>(Func<IDbCommand, T> execFunc, IDbCommand cmd = null, IDbTransaction dbTransaction = null);

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
    Task<T> ExecuteAsync<T>(Func<IDbConnection, Task<T>> execFunc);

    /// <summary>
    /// The get command.
    /// </summary>
    /// <param name="sql">
    /// The SQL Command.
    /// </param>
    /// <param name="commandType">
    /// The command type.
    /// </param>
    /// <param name="parameters">
    /// The parameters.
    /// </param>
    /// <returns>
    /// The <see cref="IDbCommand"/>.
    /// </returns>
    IDbCommand GetCommand(
        string sql,
        CommandType commandType,
        IEnumerable<KeyValuePair<string, object>> parameters = null);
}