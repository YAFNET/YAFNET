/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.Data
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    using YAF.Configuration;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    ///     The DB access base.
    /// </summary>
    public abstract class DbAccessBase : IDbAccess
    {
        #region Fields

        /// <summary>
        ///     The provider name.
        /// </summary>
        protected readonly string ProviderName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DbAccessBase"/> class.
        /// </summary>
        /// <param name="dbProviderFactory">
        /// The db provider factory. 
        /// </param>
        /// <param name="profiler">
        /// The profiler.
        /// </param>
        /// <param name="information">
        /// The information.
        /// </param>
        protected DbAccessBase(
            [NotNull] Func<string, DbProviderFactory> dbProviderFactory, IDbInformation information)
        {
            this.Information = information;
            this.DbProviderFactory = dbProviderFactory(information.ProviderName);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets ConnectionString.
        /// </summary>
        public virtual IDbInformation Information { get; protected set; }

        /// <summary>
        ///     Gets or sets DbProviderFactory.
        /// </summary>
        public virtual DbProviderFactory DbProviderFactory { get; protected set; }

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
        public virtual T Execute<T>(Func<IDbCommand, T> execFunc, IDbCommand cmd = null, IDbTransaction dbTransaction = null)
        {
            var command = cmd ?? this.GetCommand(string.Empty, CommandType.Text);

            T result;

            if (dbTransaction == null)
            {
                if (command.Connection != null && command.Connection.State == ConnectionState.Open)
                {
                    result = execFunc(command);
                }
                else
                {
                    using (var connection = this.CreateConnectionOpen())
                    {
                        // get an open connection
                        command.Connection = connection;

                        result = execFunc(command);

                        connection.Close();
                    }
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
            [NotNull] string sql, CommandType commandType = CommandType.StoredProcedure, [CanBeNull] IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            var cmd = this.DbProviderFactory.CreateCommand();
            parameters = parameters.IfNullEmpty();

            cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);
            cmd.CommandType = commandType;

            cmd.CommandText = commandType == CommandType.StoredProcedure ? this.FormatProcedureText(sql) : sql;

            // map parameters for this command...
            this.MapParameters(cmd, parameters);

            return cmd.ReplaceCommandText();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The format procedure text.
        /// </summary>
        /// <param name="functionName">
        /// The function name. 
        /// </param>
        /// <returns>
        /// The format procedure text. 
        /// </returns>
        protected virtual string FormatProcedureText(string functionName)
        {
            return $"[{{databaseOwner}}].[{{objectQualifier}}{functionName}]";
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
        protected virtual void MapParameters([NotNull] IDbCommand cmd, [NotNull] IEnumerable<KeyValuePair<string, object>> parameters)
        {
            CodeContracts.VerifyNotNull(cmd, "cmd");
            CodeContracts.VerifyNotNull(parameters, "parameters");

            // add all/any parameters...
            parameters.ForEach(cmd.AddParam);
        }

        #endregion
    }
}