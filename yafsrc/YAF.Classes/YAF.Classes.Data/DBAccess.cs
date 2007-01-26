/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2007 Jaben Cargman
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
