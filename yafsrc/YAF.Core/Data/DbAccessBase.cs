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
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Interfaces.Extensions;

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
        /// The begin transaction.
        /// </summary>
        /// <param name="isolationLevel">
        /// The isolation level. 
        /// </param>
        /// <returns>
        /// The <see cref="IDbUnitOfWork"/>.
        /// </returns>
        [NotNull]
        public virtual IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            return this.CreateConnectionOpen().BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// The execute non query.
        /// </summary>
        /// <param name="cmd">
        /// The cmd. 
        /// </param>
        /// <param name="dbTransaction">
        /// The unit of work. 
        /// </param>
        public virtual void ExecuteNonQuery([NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null)
        {
            CodeContracts.ArgumentNotNull(cmd, "cmd");

            using (var qc = new QueryCounter(cmd.CommandText))
            {
                if (dbTransaction == null)
                {
                    using (var connection = this.CreateConnectionOpen())
                    {
                        // get an open connection
                        cmd.Connection = connection;
                        cmd.ExecuteNonQuery();

                        connection.Close();
                    }
                }
                else
                {
                    cmd.Populate(dbTransaction);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="cmd">
        /// The cmd. 
        /// </param>
        /// <param name="dbTransaction">
        /// The unit of work. 
        /// </param>
        /// <returns>
        /// The execute scalar. 
        /// </returns>
        public virtual object ExecuteScalar([NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null)
        {
            CodeContracts.ArgumentNotNull(cmd, "cmd");

            using (var qc = new QueryCounter(cmd.CommandText))
            {
                object results = null;

                if (dbTransaction == null)
                {
                    using (var connection = this.CreateConnectionOpen())
                    {
                        // get an open connection
                        cmd.Connection = connection;
                        results = cmd.ExecuteScalar();

                        connection.Close();
                    }
                }
                else
                {
                    cmd.Populate(dbTransaction);

                    results = cmd.ExecuteScalar();
                }

                return results == DBNull.Value ? null : results;
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
        /// The <see cref="DbCommand"/>.
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

        /// <summary>
        /// The get data.
        /// </summary>
        /// <param name="cmd">
        /// The cmd. 
        /// </param>
        /// <param name="dbTransaction">
        /// The unit of work. 
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public virtual DataTable GetData([NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null)
        {
            CodeContracts.ArgumentNotNull(cmd, "cmd");

            using (var qc = new QueryCounter(cmd.CommandText))
            {
                return this.GetDatasetBasic(cmd, dbTransaction).Tables[0];
            }
        }

        /// <summary>
        /// The get dataset.
        /// </summary>
        /// <param name="cmd">
        /// The cmd. 
        /// </param>
        /// <param name="dbTransaction">
        /// The unit of work. 
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        [NotNull]
        public virtual DataSet GetDataset([NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null)
        {
            CodeContracts.ArgumentNotNull(cmd, "cmd");

            using (var qc = new QueryCounter(cmd.CommandText))
            {
                return this.GetDatasetBasic(cmd, dbTransaction);
            }
        }

        /// <summary>
        /// The get reader.
        /// </summary>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="readData">
        /// The read data.
        /// </param>
        /// <param name="dbTransaction">
        /// The unit of work.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public IDataReader GetReader(IDbCommand cmd, IDbTransaction dbTransaction)
        {
            CodeContracts.ArgumentNotNull(dbTransaction, "unitOfWork");
            CodeContracts.ArgumentNotNull(cmd, "cmd");

            using (var qc = new QueryCounter(cmd.CommandText))
            {
                cmd.Populate(dbTransaction);

                return cmd.ExecuteReader();
            }
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
        /// The get dataset basic.
        /// </summary>
        /// <param name="cmd">
        /// The cmd. 
        /// </param>
        /// <param name="dbTransaction">
        /// The unit of work. 
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        [NotNull]
        protected virtual DataSet GetDatasetBasic([NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null)
        {
            CodeContracts.ArgumentNotNull(cmd, "cmd");

            var ds = new DataSet();

            if (dbTransaction == null)
            {
                using (var connection = this.CreateConnectionOpen())
                {
                    // see if an existing connection is present
                    cmd.Connection = connection;

                    // create the adapter and fill....
                    IDbDataAdapter dataAdapter = this.DbProviderFactory.CreateDataAdapter();

                    if (dataAdapter != null)
                    {
                        dataAdapter.SelectCommand = cmd;
                        dataAdapter.SelectCommand.Connection = connection;
                        dataAdapter.Fill(ds);
                    }
                }
            }
            else
            {
                IDbDataAdapter dataAdapter = this.DbProviderFactory.CreateDataAdapter();

                if (dataAdapter != null)
                {
                    cmd.Populate(dbTransaction);

                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(ds);
                }
            }

            // return the dataset
            return ds;
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