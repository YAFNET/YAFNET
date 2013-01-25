/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    ///     The db access base.
    /// </summary>
    public abstract class DbAccessBase : IDbAccessV2
    {
        #region Fields

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
        public DbAccessBase(
            [NotNull] Func<string, DbProviderFactory> dbProviderFactory, [NotNull] string providerName, [NotNull] string connectionString)
        {
            this._providerName = providerName;
            this.DbProviderFactory = dbProviderFactory(providerName);
            this.ConnectionString = connectionString;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets ConnectionString.
        /// </summary>
        public virtual string ConnectionString { get; set; }

        /// <summary>
        ///     Gets DbConnectionParameters.
        /// </summary>
        public abstract IEnumerable<IDbConnectionParam> DbConnectionParameters { get; }

        /// <summary>
        ///     Gets or sets DbProviderFactory.
        /// </summary>
        public virtual DbProviderFactory DbProviderFactory { get; protected set; }

        /// <summary>
        ///     Gets FullTextScript.
        /// </summary>
        public abstract string FullTextScript { get; }

        /// <summary>
        ///     Gets ProviderName.
        /// </summary>
        public virtual string ProviderName
        {
            get
            {
                return this._providerName;
            }
        }

        /// <summary>
        ///     Gets Scripts.
        /// </summary>
        public abstract IEnumerable<string> Scripts { get; }

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
            var command = cmd ?? this.GetCommand(string.Empty, false);

            using (var qc = new QueryCounter(command.CommandText))
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

#if DEBUG
                qc.CurrentSql = command.CommandText;
#endif
                return result;
            }
        }

        /// <summary>
        /// The get command.
        /// </summary>
        /// <param name="sql">
        /// The sql. 
        /// </param>
        /// <param name="isStoredProcedure">
        /// The is stored procedure. 
        /// </param>
        /// <param name="parameters">
        /// The parameters. 
        /// </param>
        /// <returns>
        /// The <see cref="DbCommand"/> . 
        /// </returns>
        public virtual IDbCommand GetCommand(
            [NotNull] string sql, bool isStoredProcedure = true, [CanBeNull] IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            DbCommand cmd = this.DbProviderFactory.CreateCommand();
            parameters = parameters.IfNullEmpty();

            cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

            if (isStoredProcedure)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = this.FormatProcedureText(sql);
            }
            else
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
            }

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
            CodeContracts.ArgumentNotNull(cmd, "cmd");
            CodeContracts.ArgumentNotNull(parameters, "parameters");

            // add all/any parameters...
            parameters.ForEach(cmd.AddParam);
        }

        #endregion
    }
}