// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsSqlDbConnectionManager.cs" company="">
//   
// </copyright>
// <summary>
//   Provides open/close management for DB Connections
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Classes.Data
{
	#region Using

	using System;
	using System.Data;
	using System.Data.SqlClient;

	using YAF.Types;
	using YAF.Types.Handlers;
	using YAF.Types.Interfaces;
	using YAF.Utils;

	#endregion

	public static class MsSqlDbConnectionExtensions
	{
		public static SqlConnection GetOpenDbConnection(this MsSqlDbConnectionManager dbConnectionManager)
		{
			CodeContracts.ArgumentNotNull(dbConnectionManager, "dbConnectionManager");

			var connection = dbConnectionManager.DBConnection;

			if (connection.State != ConnectionState.Open)
			{
				// open it up...
				connection.Open();
			}

			return connection;			
		}
	}

	/// <summary>
	/// Provides open/close management for DB Connections
	/// </summary>
	public class MsSqlDbConnectionManager : IDbConnectionManager
	{
		#region Constants and Fields

		/// <summary>
		///   The _connection.
		/// </summary>
		protected SqlConnection _connection;

		/// <summary>
		/// The _connection string.
		/// </summary>
		private string _connectionString;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		///   Initializes a new instance of the <see cref = "MsSqlDbConnectionManager" /> class.
		/// </summary>
		public MsSqlDbConnectionManager()
		{
			this._connectionString = Config.ConnectionString;
		}

	

		#endregion

		#region Events

		/// <summary>
		///   The info message.
		/// </summary>
		public event YafDBConnInfoMessageEventHandler InfoMessage;

		#endregion

		#region Properties

		/// <summary>
		///   Gets ConnectionString.
		/// </summary>
		public virtual string ConnectionString
		{
			get
			{
				return this._connectionString;
			}

			set
			{
				this._connectionString = value;
			}
		}

		public SqlConnection DBConnection
		{
			get
			{
				this.InitConnection();
				return this._connection;				
			}
		}

		/// <summary>
		///   Gets the current DB Connection in any state.
		/// </summary>
		IDbConnection IDbConnectionManager.DBConnection
		{
			get
			{
				return this.DBConnection;
			}
		}

		#endregion

		#region Implemented Interfaces

		#region IDisposable

		/// <summary>
		/// The dispose.
		/// </summary>
		public void Dispose()
		{
			lock (this._connection)
			{
				if (this._connection != null && this._connection.State != ConnectionState.Closed)
				{
					this._connection.Close();
				}

				this._connection = null;
			}
		}

		#endregion

		#endregion

		#region Methods

		/// <summary>
		/// The connection_ info message.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Connection_InfoMessage([NotNull] object sender, [NotNull] SqlInfoMessageEventArgs e)
		{
			if (this.InfoMessage != null)
			{
				this.InfoMessage(this, new YafDBConnInfoMessageEventArgs(e.Message));
			}
		}

		/// <summary>
		/// The init connection.
		/// </summary>
		protected void InitConnection()
		{
			lock (this._connection)
			{
				if (this._connection == null)
				{
					// create the connection
					this._connection = new SqlConnection();
					this._connection.InfoMessage += this.Connection_InfoMessage;
					this._connection.ConnectionString = this.ConnectionString;
				}
				else if (this._connection.State == ConnectionState.Closed)
				{
					// verify the connection string is in there...
					this._connection.ConnectionString = this.ConnectionString;
				}
			}
		}

		#endregion
	}
}