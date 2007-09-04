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
  /// <summary>
  /// Provides open/close management for DB Connections
  /// </summary>
  public class YafDBConnManager : IDisposable
  {
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

		private static IsolationLevel _isolationLevel = IsolationLevel.ReadUncommitted;

		static public IsolationLevel IsolationLevel
		{
			get
			{
				return _isolationLevel;
			}
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
          using ( YafDBConnManager connMan = new YafDBConnManager() )
					{
            using ( SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction( _isolationLevel ) )
						{
							try
							{
								cmd.Transaction = trans;
								using ( DataSet ds = new DataSet() )
								{
									using ( SqlDataAdapter da = new SqlDataAdapter() )
									{
										da.SelectCommand = cmd;
                    da.SelectCommand.Connection = connMan.DBConnection;
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
		/// <param name="commandText">command text to be executed</param>
		/// <returns>DataTable with results</returns>
		static public DataTable GetData( string commandText )
		{
			QueryCounter qc = new QueryCounter( commandText );
			try
			{
        using ( YafDBConnManager connMan = new YafDBConnManager() )
				{
          using ( SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction( _isolationLevel ) )
					{
						try
						{
              using ( SqlCommand cmd = connMan.DBConnection.CreateCommand() )
							{
								cmd.Transaction = trans;
								cmd.CommandType = CommandType.Text;
								cmd.CommandText = commandText;
								using ( DataSet ds = new DataSet() )
								{
									using ( SqlDataAdapter da = new SqlDataAdapter() )
									{
										da.SelectCommand = cmd;
                    da.SelectCommand.Connection = connMan.DBConnection;
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
        using ( YafDBConnManager connMan = new YafDBConnManager() )
				{
          using ( SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction( _isolationLevel ) )
					{
            cmd.Connection = connMan.DBConnection;
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
        using ( YafDBConnManager connMan = new YafDBConnManager() )
				{
          using ( SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction( _isolationLevel ) )
					{
            cmd.Connection = connMan.DBConnection;
						cmd.Transaction = trans;
						object results = cmd.ExecuteScalar();
						trans.Commit();
						return results;
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
