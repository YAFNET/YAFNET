/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Types.Interfaces.Data
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    #endregion

    /// <summary>
    ///     DBAccess Interface
    /// </summary>
    public interface IDbAccess
    {
        #region Public Properties

        /// <summary>
        ///     Gets the database information
        /// </summary>
        IDbInformation Information { get; }

        /// <summary>
        ///     Gets the current db provider factory
        /// </summary>
        /// <returns> </returns>
        DbProviderFactory DbProviderFactory { get; }

        #endregion

        #region Public Methods and Operators

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
        T Execute<T>(Func<IDbCommand, T> execFunc, IDbCommand cmd = null, [CanBeNull] IDbTransaction dbTransaction = null);

        /// <summary>
        /// The get command.
        /// </summary>
        /// <param name="sql">
        /// The sql. 
        /// </param>
        /// <param name="commandType"></param>
        /// <param name="parameters">
        /// Command Parameters 
        /// </param>
        /// <returns>
        /// The <see cref="DbCommand"/> . 
        /// </returns>
        IDbCommand GetCommand(
            [NotNull] string sql,
            CommandType commandType = CommandType.StoredProcedure, 
            [CanBeNull] IEnumerable<KeyValuePair<string, object>> parameters = null);

        #endregion
    }
}