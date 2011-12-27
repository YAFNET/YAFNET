namespace YAF.Core.Data
{
	#region Using

	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Linq;

	using YAF.Classes;
	using YAF.Classes.Data;
	using YAF.Types;
	using YAF.Types.Interfaces;
	using YAF.Types.Interfaces.Extensions;
	using YAF.Utils;

	#endregion

	/// <summary>
	/// The db access base.
	/// </summary>
	public class DbAccessBase : IDbAccess
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
		public DbAccessBase([NotNull] Func<string, DbProviderFactory> dbProviderFactory, string providerName)
		{
			this._providerName = providerName;
			this.DbProviderFactory = dbProviderFactory(providerName);
			this.ConnectionString = Config.ConnectionString;
		}

		#endregion

		#region Properties

		/// <summary>
		///   Gets or sets ConnectionString.
		/// </summary>
		public virtual string ConnectionString { get; set; }

		/// <summary>
		/// Gets or sets DbProviderFactory.
		/// </summary>
		public virtual DbProviderFactory DbProviderFactory { get; protected set; }

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

		#endregion

		#region Implemented Interfaces

		#region IDbAccess

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

			using (var qc = QueryCounter.Start(cmd.CommandText))
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

			using (var qc = QueryCounter.Start(cmd.CommandText))
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
			[NotNull] string sql, 
			bool isStoredProcedure = true, 
			[CanBeNull] IEnumerable<KeyValuePair<string, object>> parameters = null)
		{
			DbCommand cmd = this.DbProviderFactory.CreateCommand();
			parameters = parameters ?? Enumerable.Empty<KeyValuePair<string, object>>();

			cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

			if (isStoredProcedure)
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "[{{databaseOwner}}].[{{objectQualifier}}{0}]".FormatWith(sql);

				if (parameters.Any() && !parameters.All(x => x.Key.IsSet()))
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = string.Format(
						"EXEC {0} {1}", 
						cmd.CommandText, 
						Enumerable.Range(0, parameters.Count()).Select(x => string.Format("@{0}", x)).ToDelimitedString(","));
				}
			}
			else
			{
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = sql;
			}

			// add all/any parameters...
			parameters.ToList().ForEach(x => cmd.AddParam(x));

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

			using (var qc = QueryCounter.Start(cmd.CommandText))
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

			using (var qc = QueryCounter.Start(cmd.CommandText))
			{
				return this.GetDatasetBasic(cmd, unitOfWork);
			}
		}

		#endregion

		#endregion

		#region Methods

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

		#endregion
	}
}