// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbAccessBase.cs" company="">
// </copyright>
// <summary>
//   The db access base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Interfaces.Extensions;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The db access base.
    /// </summary>
    public abstract class DbAccessBase : IDbAccess
    {
        #region Constants and Fields

        /// <summary>
        /// The _provider name.
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
        public DbAccessBase([NotNull] Func<string, DbProviderFactory> dbProviderFactory, [NotNull] string providerName, [NotNull] string connectionString)
        {
            this._providerName = providerName;
            this.DbProviderFactory = dbProviderFactory(providerName);
            this.ConnectionString = connectionString;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets ConnectionString.
        /// </summary>
        public virtual string ConnectionString { get; set; }

        /// <summary>
        /// Gets DbConnectionParameters.
        /// </summary>
        public abstract IEnumerable<IDbConnectionParam> DbConnectionParameters { get; }

        /// <summary>
        /// Gets or sets DbProviderFactory.
        /// </summary>
        public virtual DbProviderFactory DbProviderFactory { get; protected set; }

        /// <summary>
        /// Gets FullTextScript.
        /// </summary>
        public abstract string FullTextScript { get; }

        /// <summary>
        /// Gets ProviderName.
        /// </summary>
        public virtual string ProviderName
        {
            get
            {
                return this._providerName;
            }
        }

        /// <summary>
        /// Gets Scripts.
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
        /// </returns>
        [NotNull]
        public virtual IDbUnitOfWork BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            return new DbUnitOfWorkBase(this.CreateConnectionOpen(), isolationLevel);
        }

        /// <summary>
        /// The execute non query.
        /// </summary>
        /// <param name="cmd">
        /// The cmd. 
        /// </param>
        /// <param name="unitOfWork">
        /// The unit of work. 
        /// </param>
        public virtual void ExecuteNonQuery([NotNull] DbCommand cmd, [CanBeNull] IDbUnitOfWork unitOfWork = null)
        {
            CodeContracts.ArgumentNotNull(cmd, "cmd");

            using (var qc = new QueryCounter(cmd.CommandText))
            {
                if (unitOfWork == null)
                {
                    using (var connection = this.CreateConnectionOpen())
                    {
                        // get an open connection
                        cmd.Connection = connection;
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    unitOfWork.Setup(cmd);

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
        /// <param name="unitOfWork">
        /// The unit of work. 
        /// </param>
        /// <returns>
        /// The execute scalar. 
        /// </returns>
        public virtual object ExecuteScalar([NotNull] DbCommand cmd, [CanBeNull] IDbUnitOfWork unitOfWork = null)
        {
            CodeContracts.ArgumentNotNull(cmd, "cmd");

            using (var qc = new QueryCounter(cmd.CommandText))
            {
                object results = null;

                if (unitOfWork == null)
                {
                    using (var connection = this.CreateConnectionOpen())
                    {
                        // get an open connection
                        cmd.Connection = connection;
                        results = cmd.ExecuteScalar();
                    }
                }
                else
                {
                    unitOfWork.Setup(cmd);

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
        /// </returns>
        public virtual DbCommand GetCommand(
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
        /// <param name="unitOfWork">
        /// The unit of work. 
        /// </param>
        /// <returns>
        /// </returns>
        public virtual DataTable GetData([NotNull] DbCommand cmd, [CanBeNull] IDbUnitOfWork unitOfWork = null)
        {
            CodeContracts.ArgumentNotNull(cmd, "cmd");

            using (var qc = new QueryCounter(cmd.CommandText))
            {
                return this.GetDatasetBasic(cmd, unitOfWork).Tables[0];
            }
        }

        /// <summary>
        /// The get dataset.
        /// </summary>
        /// <param name="cmd">
        /// The cmd. 
        /// </param>
        /// <param name="unitOfWork">
        /// The unit of work. 
        /// </param>
        /// <returns>
        /// </returns>
        [NotNull]
        public virtual DataSet GetDataset([NotNull] DbCommand cmd, [CanBeNull] IDbUnitOfWork unitOfWork = null)
        {
            CodeContracts.ArgumentNotNull(cmd, "cmd");

            using (var qc = new QueryCounter(cmd.CommandText))
            {
                return this.GetDatasetBasic(cmd, unitOfWork);
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
        /// <param name="unitOfWork">
        /// The unit of work. 
        /// </param>
        /// <returns>
        /// </returns>
        [NotNull]
        protected virtual DataSet GetDatasetBasic([NotNull] DbCommand cmd, [CanBeNull] IDbUnitOfWork unitOfWork = null)
        {
            CodeContracts.ArgumentNotNull(cmd, "cmd");

            var ds = new DataSet();

            if (unitOfWork == null)
            {
                using (var connection = this.CreateConnectionOpen())
                {
                    // see if an existing connection is present
                    cmd.Connection = connection;

                    // create the adapter and fill....
                    using (var da = this.DbProviderFactory.CreateDataAdapter())
                    {
                        da.SelectCommand = cmd;
                        da.SelectCommand.Connection = connection;
                        da.Fill(ds);
                    }
                }
            }
            else
            {
                // create the adapter and fill...
                using (var da = this.DbProviderFactory.CreateDataAdapter())
                {
                    da.SelectCommand = cmd;

                    unitOfWork.Setup(da.SelectCommand);

                    da.Fill(ds);
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
        protected virtual void MapParameters([NotNull] DbCommand cmd, [NotNull] IEnumerable<KeyValuePair<string, object>> parameters)
        {
            CodeContracts.ArgumentNotNull(cmd, "cmd");
            CodeContracts.ArgumentNotNull(parameters, "parameters");

            // add all/any parameters...
            parameters.ForEach(cmd.AddParam);
        }

        #endregion
    }
}