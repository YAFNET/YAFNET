// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsSqlDbAccess.cs" company="">
//   
// </copyright>
// <summary>
//   The yaf db access for SQL Server.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Classes.Data
{
	#region Using

	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Linq;

	using YAF.Types;
	using YAF.Types.Interfaces;
	using YAF.Types.Interfaces.Extensions;
	using YAF.Utils;
	using YAF.Utils.Helpers;

	#endregion

	/// <summary>
	/// The yaf db access for SQL Server.
	/// </summary>
	public class MsSqlDbAccess : DbAccessBase
	{
		public MsSqlDbAccess(IDbFactoryProvider factoryProvider)
		{
			this._factoryProvider = factoryProvider;
		}

		#region Constants and Fields

		/// <summary>
		///   Result filter list
		/// </summary>
		private readonly IList<IDataTableResultFilter> _resultFilterList = new List<IDataTableResultFilter>();

		/// <summary>
		///   The _isolation level.
		/// </summary>
		private static IsolationLevel _isolationLevel = IsolationLevel.ReadUncommitted;

		/// <summary>
		/// The _connection manager.
		/// </summary>
		private IDbFactoryProvider _factoryProvider = new MsSqlDbConnectionProvider();

		#endregion

		#region Properties

		/// <summary>
		///   Gets IsolationLevel.
		/// </summary>
		public static IsolationLevel IsolationLevel
		{
			get
			{
				return _isolationLevel;
			}
		}

		/// <summary>
		///   The get connection manager.
		/// </summary>
		/// <returns>
		/// </returns>
		[NotNull]
		public IDbFactoryProvider FactoryProvider
		{
			get
			{
				return this._factoryProvider;
			}

			set
			{
				if (value != null)
				{
					this._factoryProvider = value;
				}
			}
		}

		/// <summary>
		///   Gets the Result Filter List.
		/// </summary>
		/// <exception cref = "NotImplementedException">
		/// </exception>
		public IList<IDataTableResultFilter> ResultFilterList
		{
			get
			{
				return this._resultFilterList;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a Connection String from the parameters.
		/// </summary>
		/// <param name="parm1">
		/// </param>
		/// <param name="parm2">
		/// </param>
		/// <param name="parm3">
		/// </param>
		/// <param name="parm4">
		/// </param>
		/// <param name="parm5">
		/// </param>
		/// <param name="parm6">
		/// </param>
		/// <param name="parm7">
		/// </param>
		/// <param name="parm8">
		/// </param>
		/// <param name="parm9">
		/// </param>
		/// <param name="parm10">
		/// </param>
		/// <param name="parm11">
		/// </param>
		/// <param name="parm12">
		/// </param>
		/// <param name="parm13">
		/// </param>
		/// <param name="parm14">
		/// </param>
		/// <param name="parm15">
		/// </param>
		/// <param name="parm16">
		/// </param>
		/// <param name="parm17">
		/// </param>
		/// <param name="parm18">
		/// </param>
		/// <param name="parm19">
		/// </param>
		/// <param name="userID">
		/// </param>
		/// <param name="userPassword">
		/// </param>
		/// <returns>
		/// The get connection string.
		/// </returns>
		public static string GetConnectionString(
			[NotNull] string parm1, 
			[NotNull] string parm2, 
			[NotNull] string parm3, 
			[NotNull] string parm4, 
			[NotNull] string parm5, 
			[NotNull] string parm6, 
			[NotNull] string parm7, 
			[NotNull] string parm8, 
			[NotNull] string parm9, 
			[NotNull] string parm10, 
			bool parm11, 
			bool parm12, 
			bool parm13, 
			bool parm14, 
			bool parm15, 
			bool parm16, 
			bool parm17, 
			bool parm18, 
			bool parm19, 
			[NotNull] string userID, 
			[NotNull] string userPassword)
		{
			// TODO: Parameters should be in a List<ConnectionParameters>
			var connBuilder = new SqlConnectionStringBuilder { DataSource = parm1, InitialCatalog = parm2 };

			if (parm11)
			{
				connBuilder.IntegratedSecurity = true;
			}
			else
			{
				connBuilder.UserID = userID;
				connBuilder.Password = userPassword;
			}

			return connBuilder.ConnectionString;
		}

		/// <summary>
		/// Gets qualified object name
		/// </summary>
		/// <param name="name">
		/// Base name of an object
		/// </param>
		/// <returns>
		/// Returns qualified object name of format {databaseOwner}.{objectQualifier}name
		/// </returns>
		public static string GetObjectName([NotNull] string name)
		{
			return "[{0}].[{1}{2}]".FormatWith(Config.DatabaseOwner, Config.DatabaseObjectQualifier, name);
		}

		#endregion

		#region Implemented Interfaces

		#region IDbAccess

		/// <summary>
		/// The execute non query.
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		public void ExecuteNonQuery([NotNull] IDbCommand cmd, bool transaction)
		{
			using (var qc = QueryCounter.Start(cmd.CommandText))
			{
				using (var connection = this.FactoryProvider.GetOpenDbConnection())
				{
					// get an open connection
					cmd.Connection = connection;

					Trace.WriteLine(cmd.ToDebugString(), "DbAccess");

					if (transaction)
					{
						// execute using a transaction
						using (var trans = connection.BeginTransaction(_isolationLevel))
						{
							cmd.Transaction = trans;
							cmd.ExecuteNonQuery();
							trans.Commit();
						}
					}
					else
					{
						// don't use a transaction
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		/// <summary>
		/// The execute scalar.
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <returns>
		/// The execute scalar.
		/// </returns>
		public object ExecuteScalar([NotNull] IDbCommand cmd, bool transaction)
		{
			using (var qc = QueryCounter.Start(cmd.CommandText))
			{
				using (var connection = this.FactoryProvider.GetOpenDbConnection())
				{
					// get an open connection
					cmd.Connection = connection;

					Trace.WriteLine(cmd.ToDebugString(), "DbAccess");

					object results;

					if (transaction)
					{
						// get scalar using a transaction
						using (var trans = connection.BeginTransaction(_isolationLevel))
						{
							cmd.Transaction = trans;
							results = cmd.ExecuteScalar();
							trans.Commit();
						}
					}
					else
					{
						// get scalar regular
						results = cmd.ExecuteScalar();
					}

					return results == DBNull.Value ? null : results;
				}
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
		public DbCommand GetCommand(
			[NotNull] string sql, 
			bool isStoredProcedure = true, 
			[CanBeNull] IEnumerable<KeyValuePair<string, object>> parameters = null)
		{
			SqlCommand cmd = null;
			parameters = parameters ?? Enumerable.Empty<KeyValuePair<string, object>>();

			if (isStoredProcedure)
			{
				cmd = new SqlCommand
					{
						CommandType = CommandType.StoredProcedure, 
						CommandText = "[{{databaseOwner}}].[{{objectQualifier}}{0}]".FormatWith(sql), 
						CommandTimeout = int.Parse(Config.SqlCommandTimeout)
					};

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
				cmd = new SqlCommand
					{
				CommandType = CommandType.Text, CommandText = sql, CommandTimeout = int.Parse(Config.SqlCommandTimeout) 
		 };
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
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <returns>
		/// </returns>
		public DataTable GetData([NotNull] IDbCommand cmd, bool transaction)
		{
			using (var qc = QueryCounter.Start(cmd.CommandText))
			{
				return this.ProcessUsingResultFilters(this.GetDatasetBasic(cmd, transaction).Tables[0], cmd.CommandText);
			}
		}

		/// <summary>
		/// The get data.
		/// </summary>
		/// <param name="commandText">
		/// The command text.
		/// </param>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <returns>
		/// </returns>
		public DataTable GetData([NotNull] string commandText, bool transaction)
		{
			using (var qc = QueryCounter.Start(commandText))
			{
				using (var cmd = new SqlCommand())
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = commandText;
					return this.ProcessUsingResultFilters(this.GetDatasetBasic(cmd, transaction).Tables[0], commandText);
				}
			}
		}

		/// <summary>
		/// The get dataset.
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <returns>
		/// </returns>
		[NotNull]
		public DataSet GetDataset([NotNull] IDbCommand cmd, bool transaction)
		{
			using (var qc = QueryCounter.Start(cmd.CommandText))
			{
				return this.GetDatasetBasic(cmd, transaction);
			}
		}

		#endregion

		#endregion

		#region Methods

		/// <summary>
		/// Used internally to get data for all the other functions
		/// </summary>
		/// <param name="cmd">
		/// </param>
		/// <param name="transaction">
		/// </param>
		/// <returns>
		/// </returns>
		[NotNull]
		private DataSet GetDatasetBasic([NotNull] IDbCommand cmd, bool transaction)
		{
			using (var connection = this.FactoryProvider.GetOpenDbConnection())
			{
				// see if an existing connection is present
				cmd.Connection = connection;

				// create the adapters
				using (var ds = new DataSet())
				{
					using (var da = new SqlDataAdapter())
					{
						da.SelectCommand = (SqlCommand)cmd;
						da.SelectCommand.Connection = (SqlConnection)cmd.Connection;
						Trace.WriteLine(cmd.ToDebugString(), "DbAccess");

						// use a transaction
						if (transaction)
						{
							using (var trans = connection.BeginTransaction(_isolationLevel))
							{
								try
								{
									da.SelectCommand.Transaction = (SqlTransaction)trans;
									da.Fill(ds);
								}
								finally
								{
									trans.Commit();
								}
							}
						}
						else
						{
							// no transaction
							da.Fill(ds);
						}

						// return the dataset
						return ds;
					}
				}
			}
		}

		/// <summary>
		/// Process a <see cref="DataTable"/> using Result Filters.
		/// </summary>
		/// <param name="dataTable">
		/// data table to process
		/// </param>
		/// <param name="sqlCommand">
		/// </param>
		/// <returns>
		/// </returns>
		private DataTable ProcessUsingResultFilters([NotNull] DataTable dataTable, [NotNull] string sqlCommand)
		{
			string commandCleaned =
				sqlCommand.Replace("[{0}].[{1}".FormatWith(Config.DatabaseOwner, Config.DatabaseObjectQualifier), string.Empty);

			if (commandCleaned.EndsWith("]"))
			{
				// remove last character
				commandCleaned = commandCleaned.Substring(0, commandCleaned.Length - 1);
			}

			// sort filters and process each one...
			this.ResultFilterList.OrderBy(x => x.Rank).ToList().ForEach(i => i.Process(ref dataTable, commandCleaned));

			// return possibility modified dataTable
			return dataTable;
		}

		#endregion
	}
}