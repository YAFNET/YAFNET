/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	public partial class reindex : YAF.Classes.Base.AdminPage
	{
		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !PageContext.IsHostAdmin )
			{
				YafBuildLink.AccessDenied();
			}

			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin ) );
				PageLinks.AddLink( "Reindex DB", "" );

				BindData();
			}
		}

		private void BindData()
		{
			DataBind();
		}

		protected void btnGetStats_Click( object sender, EventArgs e )
		{
			// create statistic getting SQL...
			StringBuilder sb = new StringBuilder();

			sb.AppendLine( "DECLARE @TableName sysname");
			sb.AppendLine( "DECLARE cur_showfragmentation CURSOR FOR" );
			sb.AppendFormat( "SELECT table_name FROM information_schema.tables WHERE table_type = 'base table' AND table_name LIKE '{0}%'", DBAccess.ObjectQualifier );
			sb.AppendLine( "OPEN cur_showfragmentation");
			sb.AppendLine( "FETCH NEXT FROM cur_showfragmentation INTO @TableName");
			sb.AppendLine( "WHILE @@FETCH_STATUS = 0");
			sb.AppendLine( "BEGIN");
			sb.AppendLine( "DBCC SHOWCONTIG (@TableName)");
			sb.AppendLine( "FETCH NEXT FROM cur_showfragmentation INTO @TableName");
			sb.AppendLine( "END");
			sb.AppendLine( "CLOSE cur_showfragmentation");
			sb.AppendLine( "DEALLOCATE cur_showfragmentation" );

			using ( YafDBConnManager connMan = new YafDBConnManager() )
			{
				connMan.DBConnection.InfoMessage += new SqlInfoMessageEventHandler( DBConnection_InfoMessage );

				using ( SqlCommand cmd = new SqlCommand( sb.ToString(), connMan.OpenDBConnection ) )
				{
					cmd.Connection = connMan.DBConnection;
					// up the command timeout...
					cmd.CommandTimeout = 9999;
					// run it...
					cmd.ExecuteNonQuery();
				}
			}		
		}

		protected void btnReindex_Click( object sender, EventArgs e )
		{
			// create statistic getting SQL...
			StringBuilder sb = new StringBuilder();

			sb.AppendLine( "DECLARE @MyTable VARCHAR(255)" );
			sb.AppendLine( "DECLARE myCursor" );
			sb.AppendLine( "CURSOR FOR" );
			sb.AppendFormat( "SELECT table_name FROM information_schema.tables WHERE table_type = 'base table' AND table_name LIKE '{0}%'", DBAccess.ObjectQualifier );
			sb.AppendLine( "OPEN myCursor" );
			sb.AppendLine( "FETCH NEXT" );
			sb.AppendLine( "FROM myCursor INTO @MyTable" );
			sb.AppendLine( "WHILE @@FETCH_STATUS = 0" );
			sb.AppendLine( "BEGIN" );
			sb.AppendLine( "PRINT 'Reindexing Table:  ' + @MyTable" );
			sb.AppendLine( "DBCC DBREINDEX(@MyTable, '', 80)" );
			sb.AppendLine( "FETCH NEXT" );
			sb.AppendLine( "FROM myCursor INTO @MyTable" );
			sb.AppendLine( "END" );
			sb.AppendLine( "CLOSE myCursor" );
			sb.AppendLine( "DEALLOCATE myCursor" );
			sb.AppendLine( "EXEC sp_updatestats" );

			using ( YafDBConnManager connMan = new YafDBConnManager() )
			{
				connMan.DBConnection.InfoMessage += new SqlInfoMessageEventHandler( DBConnection_InfoMessage );

				using ( SqlCommand cmd = new SqlCommand( sb.ToString(), connMan.OpenDBConnection ) )
				{
					cmd.Connection = connMan.DBConnection;
					// up the command timeout...
					cmd.CommandTimeout = 9999;
					// run it...
					cmd.ExecuteNonQuery();
				}
			}
		}

		void DBConnection_InfoMessage( object sender, SqlInfoMessageEventArgs e )
		{
			txtIndexStatistics.Text = e.Message;
		}
	}
}
