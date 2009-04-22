/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace YAF.Classes.Data
{
	/// <summary>
	/// Provides open/close management for DB Connections
	/// </summary>
	public class YafDBConnManager : IDisposable
	{
		public class YafDBConnInfoMessageEventArgs : EventArgs
		{
			private string _message;

			public string Message
			{
				get
				{
					return _message;
				}
				set
				{
					_message = value;
				}
			}

			public YafDBConnInfoMessageEventArgs( string message )
			{
				Message = message;
			}
		}

		public delegate void YafDBConnInfoMessageEventHandler( object sender, YafDBConnInfoMessageEventArgs e );

		public event YafDBConnInfoMessageEventHandler InfoMessage;

		private SqlConnection _connection = null;

		public YafDBConnManager()
		{
			// just initalize it (not open)
			InitConnection();
		}

		public void InitConnection()
		{
			if ( _connection == null )
			{
				// create the connection
				_connection = new SqlConnection();
				_connection.InfoMessage += new SqlInfoMessageEventHandler( Connection_InfoMessage );
				_connection.ConnectionString = YAF.Classes.Config.ConnectionString;
			}
			else if ( _connection.State != ConnectionState.Open )
			{
				// verify the connection string is in there...
				_connection.ConnectionString = YAF.Classes.Config.ConnectionString;
			}

		}

		public void CloseConnection()
		{
			if ( _connection != null && _connection.State != ConnectionState.Closed )
			{
				_connection.Close();
			}
		}

		void Connection_InfoMessage( object sender, SqlInfoMessageEventArgs e )
		{
			if ( InfoMessage != null )
			{
				InfoMessage( this, new YafDBConnInfoMessageEventArgs( e.Message ) );
			}
		}

		/// <summary>
		/// Gets the current DB Connection in any state.
		/// </summary>
		public SqlConnection DBConnection
		{
			get
			{
				InitConnection();
				return _connection;
			}
		}

		/// <summary>
		/// Gets an open connection to the DB. Can be called any number of times.
		/// </summary>
		public SqlConnection OpenDBConnection
		{
			get
			{
				InitConnection();

				if ( _connection.State != ConnectionState.Open )
				{
					// open it up...
					_connection.Open();
				}

				return _connection;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			// close and delete connection
			CloseConnection();
			_connection = null;
		}

		#endregion
	}

	public static class DBAccess
	{
		/* Ederon : 6/16/2007 - conventions */
		private const IsolationLevel _isolationLevel = IsolationLevel.ReadUncommitted;
		private static string _dbOwner;
		private static string _objectQualifier;

		static public IsolationLevel IsolationLevel
		{
			get
			{
				return _isolationLevel;
			}
		}

		static public string DatabaseOwner
		{
			get
			{
				if ( _dbOwner == null )
				{
					_dbOwner = Config.GetConfigValueAsString( "YAF.DatabaseOwner" );
				}

				return _dbOwner;
			}
		}

		static public string ObjectQualifier
		{
			get
			{
				if ( _objectQualifier == null )
				{
					_objectQualifier = Config.GetConfigValueAsString( "YAF.DatabaseObjectQualifier" );
				}

				return _objectQualifier;
			}
		}


		/// <summary>
		/// Creates new SqlCommand based on command text applying all qualifiers to the name.
		/// </summary>
		/// <param name="commandText">Command text to qualify.</param>
		/// <param name="isText">Determines whether command text is text or stored procedure.</param>
		/// <returns>New SqlCommand</returns>
		static public SqlCommand GetCommand( string commandText, bool isText )
		{

			return GetCommand( commandText, isText, null );
		}
		/// <summary>
		/// Creates new SqlCommand based on command text applying all qualifiers to the name.
		/// </summary>
		/// <param name="commandText">Command text to qualify.</param>
		/// <param name="isText">Determines whether command text is text or stored procedure.</param>
		/// <param name="connection">Connection to use with command.</param>
		/// <returns>New SqlCommand</returns>
		static public SqlCommand GetCommand( string commandText, bool isText, SqlConnection connection )
		{
			if ( isText )
			{
				commandText = commandText.Replace( "{databaseOwner}", DatabaseOwner );
				commandText = commandText.Replace( "{objectQualifier}", ObjectQualifier );

				SqlCommand cmd = new SqlCommand();

				cmd.CommandType = CommandType.Text;
				cmd.CommandText = commandText;
				cmd.Connection = connection;

				return cmd;
			}
			else
			{
				return GetCommand( commandText );
			}
		}
		/// <summary>
		/// Creates new SqlCommand calling stored procedure applying all qualifiers to the name.
		/// </summary>
		/// <param name="storedProcedure">Base of stored procedure name.</param>
		/// <returns>New SqlCommand</returns>
		static public SqlCommand GetCommand( string storedProcedure )
		{
			return GetCommand( storedProcedure, null );
		}
		/// <summary>
		/// Creates new SqlCommand calling stored procedure applying all qualifiers to the name.
		/// </summary>
		/// <param name="storedProcedure">Base of stored procedure name.</param>
		/// <param name="connection">Connection to use with command.</param>
		/// <returns>New SqlCommand</returns>
		static public SqlCommand GetCommand( string storedProcedure, SqlConnection connection )
		{
			SqlCommand cmd = new SqlCommand();

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = GetObjectName( storedProcedure );
			cmd.Connection = connection;

			return cmd;
		}

		/// <summary>
		/// Gets qualified object name
		/// </summary>
		/// <param name="name">Base name of an object</param>
		/// <returns>Returns qualified object name of format {databaseOwner}.{objectQualifier}name</returns>
		static public string GetObjectName( string name )
		{
			return String.Format(
							"[{0}].[{1}{2}]",
							DatabaseOwner,
							ObjectQualifier,
							name
							);
		}

		/// <summary>
		/// Gets a whole dataset out of the database
		/// </summary>
		/// <param name="cmd">The SQL Command</param>
		/// <returns>Dataset with the results</returns>
		/// <remarks>Without transaction.</remarks>
		static public DataSet GetDataset( SqlCommand cmd )
		{
			return GetDataset( cmd, false );
		}
		static public DataSet GetDataset( SqlCommand cmd, bool transaction )
		{
			QueryCounter qc = new QueryCounter( cmd.CommandText );

			try
			{
				return GetDatasetBasic( cmd, transaction );
			}
			finally
			{
				qc.Dispose();
			}
		}

		/// <summary>
		/// Used internally to get data for all the other functions
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		static public DataSet GetDatasetBasic( SqlCommand cmd, bool transaction )
		{
			using ( YafDBConnManager connMan = new YafDBConnManager() )
			{
				// see if an existing connection is present
				if ( cmd.Connection == null )
				{
					cmd.Connection = connMan.OpenDBConnection;
				}
				else if ( cmd.Connection != null && cmd.Connection.State != ConnectionState.Open )
				{
					cmd.Connection.Open();
				}

				// create the adapters
				using ( DataSet ds = new DataSet() )
				{
					using ( SqlDataAdapter da = new SqlDataAdapter() )
					{
						da.SelectCommand = cmd;
						da.SelectCommand.Connection = cmd.Connection;

						// use a transaction
						if ( transaction )
						{
							using ( SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction( _isolationLevel ) )
							{
								try
								{
									da.SelectCommand.Transaction = trans;
									da.Fill( ds );
								}
								finally
								{
									trans.Commit();
								}
							}
						}
						else // no transaction
						{
							da.Fill( ds );
						}

						// return the dataset
						return ds;
					}
				}
			}
		}

		/// <summary>
		/// Gets data out of the database
		/// </summary>
		/// <param name="cmd">The SQL Command</param>
		/// <returns>DataTable with the results</returns>
		/// <remarks>Without transaction.</remarks>
		static public DataTable GetData( SqlCommand cmd )
		{
			return GetData( cmd, false );
		}
		static public DataTable GetData( SqlCommand cmd, bool transaction )
		{
			QueryCounter qc = new QueryCounter( cmd.CommandText );

			try
			{
				return GetDatasetBasic( cmd, transaction ).Tables [0];
			}
			finally
			{
				qc.Dispose();
			}
		}

		/// <summary>
		/// Gets data out of database using a plain text string command
		/// </summary>
		/// <param name="commandText">command text to be executed</param>
		/// <returns>DataTable with results</returns>
		/// <remarks>Without transaction.</remarks>
		static public DataTable GetData( string commandText )
		{
			return GetData( commandText, false );
		}
		static public DataTable GetData( string commandText, bool transaction )
		{
			QueryCounter qc = new QueryCounter( commandText );
			try
			{
				using ( SqlCommand cmd = new SqlCommand() )
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = commandText;
					return GetDatasetBasic( cmd, transaction ).Tables [0];
				}
			}
			finally
			{
				qc.Dispose();
			}
		}

		/// <summary>
		/// Executes a NonQuery
		/// </summary>
		/// <param name="cmd">NonQuery to execute</param>
		/// <remarks>Without transaction</remarks>
		static public void ExecuteNonQuery( SqlCommand cmd )
		{
			// defaults to using a transaction for non-queries
			ExecuteNonQuery( cmd, true );
		}
		static public void ExecuteNonQuery( SqlCommand cmd, bool transaction )
		{
			QueryCounter qc = new QueryCounter( cmd.CommandText );
			try
			{
				using ( YafDBConnManager connMan = new YafDBConnManager() )
				{
					// get an open connection
					cmd.Connection = connMan.OpenDBConnection;

					if ( transaction )
					{
						// execute using a transaction
						using ( SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction( _isolationLevel ) )
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
			finally
			{
				qc.Dispose();
			}
		}


		static public object ExecuteScalar( SqlCommand cmd )
		{
			// default to using a transaction for scaler commands
			return ExecuteScalar( cmd, true );
		}
		static public object ExecuteScalar( SqlCommand cmd, bool transaction )
		{
			QueryCounter qc = new QueryCounter( cmd.CommandText );
			try
			{
				using ( YafDBConnManager connMan = new YafDBConnManager() )
				{
					// get an open connection
					cmd.Connection = connMan.OpenDBConnection;

					if ( transaction )
					{
						// get scalar using a transaction
						using ( SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction( _isolationLevel ) )
						{
							cmd.Transaction = trans;
							object results = cmd.ExecuteScalar();
							trans.Commit();
							return results;
						}
					}
					else
					{
						// get scalar regular
						return cmd.ExecuteScalar();
					}
				}
			}
			finally
			{
				qc.Dispose();
			}
		}



	}

	/// <summary>
	/// Helper class to do basic data conversion for a DataRow.	
	/// </summary>
	public class DataRowConvert
	{
		private DataRow _dbRow;

		public DataRowConvert( DataRow dbRow )
		{
			_dbRow = dbRow;
		}

		public string AsString( string columnName )
		{
			if ( _dbRow [columnName] == DBNull.Value ) return null;
			return _dbRow [columnName].ToString();
		}

		public bool AsBool( string columnName )
		{
			if ( _dbRow [columnName] == DBNull.Value ) return false;
			return Convert.ToBoolean( _dbRow [columnName] );
		}

		public DateTime? AsDateTime( string columnName )
		{
			if ( _dbRow [columnName] == DBNull.Value ) return null;
			return Convert.ToDateTime( _dbRow [columnName] );
		}

		public int? AsInt32( string columnName )
		{
			if ( _dbRow [columnName] == DBNull.Value ) return null;
			return Convert.ToInt32( _dbRow [columnName] );
		}

		public long? AsInt64( string columnName )
		{
			if ( _dbRow [columnName] == DBNull.Value ) return null;
			return Convert.ToInt64( _dbRow [columnName] );
		}
	}

	public static class SqlDataLayerConverter
	{
		public static int VerifyInt32( object o )
		{
			return Convert.ToInt32( o );
		}
		public static bool VerifyBool( object o )
		{
			return Convert.ToBoolean( o );
		}
	}
}
