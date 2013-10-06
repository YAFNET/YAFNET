/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Core.Data
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    using QueryCounter = YAF.Core.Data.Profiling.QueryCounter;

    #endregion

    /// <summary>
    ///     The db access base.
    /// </summary>
    public abstract class DbAccessBase : IDbAccess
    {
        #region Fields

        private readonly IProfileQuery _profiler;

        /// <summary>
        ///     The _provider name.
        /// </summary>
        protected readonly string _providerName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DbAccessBase"/> class.
        /// </summary>
        /// <param name="dbProviderFactory">
        /// The db provider factory. 
        /// </param>
        /// <param name="providerName">
        /// The provider name. 
        /// </param>
        /// <param name="connectionString">
        /// The connection String. 
        /// </param>
        protected DbAccessBase(
            [NotNull] Func<string, DbProviderFactory> dbProviderFactory, IProfileQuery profiler, IDbInformation information)
        {
            this.Information = information;
            this._profiler = profiler;
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

            using (var p = this._profiler.Start(command.CommandText))
            {
                T result = default(T);

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
            DbCommand cmd = this.DbProviderFactory.CreateCommand();
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
            return "[{{databaseOwner}}].[{{objectQualifier}}{0}]".FormatWith(functionName);
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