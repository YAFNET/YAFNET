using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace YAF.Classes.Data
{
	public static class DBAccess
	{
		private static IsolationLevel m_isoLevel = IsolationLevel.ReadUncommitted;

		static public IsolationLevel IsolationLevel
		{
			get
			{
				return m_isoLevel;
			}
		}

		/// <summary>
		/// Gets Connection out of Web.config
		/// </summary>
		/// <returns>Returns SqlConnection</returns>
		public static SqlConnection GetConnection()
		{
			SqlConnection conn = new SqlConnection( YAF.Classes.Config.ConnectionString );
			conn.Open();
			return conn;
		}
		/// <summary>
		/// Gets data out of the database
		/// </summary>
		/// <param name="cmd">The SQL Command</param>
		/// <returns>DataTable with the results</returns>
		static public DataTable GetData( SqlCommand cmd )
		{
			QueryCounter qc = new QueryCounter( cmd.CommandText );
			try
			{
				if ( cmd.Connection != null )
				{
					using ( DataSet ds = new DataSet() )
					{
						using ( SqlDataAdapter da = new SqlDataAdapter() )
						{
							da.SelectCommand = cmd;
							da.Fill( ds );
							return ds.Tables [0];
						}
					}
				}
				else
				{
					using ( SqlConnection conn = GetConnection() )
					{
						using ( SqlTransaction trans = conn.BeginTransaction( m_isoLevel ) )
						{
							try
							{
								cmd.Transaction = trans;
								using ( DataSet ds = new DataSet() )
								{
									using ( SqlDataAdapter da = new SqlDataAdapter() )
									{
										da.SelectCommand = cmd;
										da.SelectCommand.Connection = conn;
										da.Fill( ds );
										return ds.Tables [0];
									}
								}
							}
							finally
							{
								trans.Commit();
							}
						}
					}
				}
			}
			finally
			{
				qc.Dispose();
			}
		}
		/// <summary>
		/// Gets data out of database using a plain text string command
		/// </summary>
		/// <param name="sql">string command to be executed</param>
		/// <returns>DataTable with results</returns>
		static public DataTable GetData( string sql )
		{
			QueryCounter qc = new QueryCounter( sql );
			try
			{
				using ( SqlConnection conn = GetConnection() )
				{
					using ( SqlTransaction trans = conn.BeginTransaction( m_isoLevel ) )
					{
						try
						{
							using ( SqlCommand cmd = conn.CreateCommand() )
							{
								cmd.Transaction = trans;
								cmd.CommandType = CommandType.Text;
								cmd.CommandText = sql;
								using ( DataSet ds = new DataSet() )
								{
									using ( SqlDataAdapter da = new SqlDataAdapter() )
									{
										da.SelectCommand = cmd;
										da.SelectCommand.Connection = conn;
										da.Fill( ds );
										return ds.Tables [0];
									}
								}
							}
						}
						finally
						{
							trans.Commit();
						}
					}
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
		static public void ExecuteNonQuery( SqlCommand cmd )
		{
			QueryCounter qc = new QueryCounter( cmd.CommandText );
			try
			{
				using ( SqlConnection conn = GetConnection() )
				{
					using ( SqlTransaction trans = conn.BeginTransaction( m_isoLevel ) )
					{
						cmd.Connection = conn;
						cmd.Transaction = trans;
						cmd.ExecuteNonQuery();
						trans.Commit();
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
			QueryCounter qc = new QueryCounter( cmd.CommandText );
			try
			{
				using ( SqlConnection conn = GetConnection() )
				{
					using ( SqlTransaction trans = conn.BeginTransaction( m_isoLevel ) )
					{
						cmd.Connection = conn;
						cmd.Transaction = trans;
						object res = cmd.ExecuteScalar();
						trans.Commit();
						return res;
					}
				}
			}
			finally
			{
				qc.Dispose();
			}
		}
	}
}
