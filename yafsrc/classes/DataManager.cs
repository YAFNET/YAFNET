/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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
using System.Data;
using System.Data.SqlClient;

namespace yaf
{
	public class DataManager
	{
		public static SqlConnection GetConnection() 
		{
			try 
			{
				SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
				conn.Open();
				return conn;
			}
			catch(Exception) 
			{
				return null;
			}
		}

		public static DataTable GetData(string sql,CommandType cmdtype) 
		{
			using(SqlConnection conn = GetConnection()) 
			{
				using(DataSet ds = new DataSet()) 
				{
					using(SqlDataAdapter da = new SqlDataAdapter(sql,conn)) 
					{
						da.SelectCommand.CommandType = cmdtype;
						da.Fill(ds);
						return ds.Tables[0];
					}
				}
			}
		}

		public static DataTable GetData(SqlCommand cmd) 
		{
			if(cmd.Connection!=null) 
			{
				using(DataSet ds = new DataSet()) 
				{
					using(SqlDataAdapter da = new SqlDataAdapter()) 
					{
						da.SelectCommand = cmd;
						da.Fill(ds);
						return ds.Tables[0];
					}
				}
			} 
			else 
			{
				using(SqlConnection conn = GetConnection()) 
				{
					using(DataSet ds = new DataSet()) 
					{
						using(SqlDataAdapter da = new SqlDataAdapter()) 
						{
							da.SelectCommand = cmd;
							da.SelectCommand.Connection = conn;
							da.Fill(ds);
							return ds.Tables[0];
						}
					}
				}
			}
		}

		public static void ExecuteNonQuery(string sql,CommandType cmdtype) 
		{
			using(SqlConnection conn = GetConnection()) 
			{
				using(SqlCommand cmd = new SqlCommand(sql,conn)) 
				{
					cmd.CommandType = cmdtype;
					cmd.ExecuteNonQuery();
				}
			}
		}

		public static void ExecuteNonQuery(SqlCommand cmd) 
		{
			using(SqlConnection conn = GetConnection()) 
			{
				cmd.Connection = conn;
				cmd.ExecuteNonQuery();
			}
		}
	
		public static object ExecuteScalar(string sql,CommandType cmdtype) 
		{
			using(SqlConnection conn = GetConnection()) 
			{
				using(SqlCommand cmd = new SqlCommand(sql,conn)) 
				{
					cmd.CommandType = cmdtype;
					return cmd.ExecuteScalar();
				}
			}
		}

		public static object ExecuteScalar(SqlCommand cmd) 
		{
			using(SqlConnection conn = GetConnection()) 
			{
				cmd.Connection = conn;
				return cmd.ExecuteScalar();
			}
		}
	}
}
