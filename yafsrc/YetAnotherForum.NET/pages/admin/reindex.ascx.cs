/* Yet Another Forum.NET
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
using YAF.Classes;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	public partial class reindex : YAF.Classes.Core.AdminPage
	{
		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !PageContext.IsHostAdmin )
			{
				YafBuildLink.AccessDenied();
			}

			if ( !IsPostBack )
			{
				//Check and see if it should make panels enable or not
				this.PanelReindex.Visible = DB.PanelReindex;
				this.PanelShrink.Visible = DB.PanelShrink;
				this.PanelRecoveryMode.Visible = DB.PanelRecoveryMode;
				this.PanelGetStats.Visible = DB.PanelGetStats;

				//Get the name of buttons
				this.btnReindex.Text = DB.btnReindexName;
				this.btnGetStats.Text = DB.btnGetStatsName;
				this.btnShrink.Text = DB.btnShrinkName;
				this.btnRecoveryMode.Text = DB.btnRecoveryModeName;

				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YafBuildLink.GetLink( ForumPages.admin_admin ) );
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

			using ( YafDBConnManager connMan = new YafDBConnManager() )
			{
				connMan.InfoMessage += new YafDBConnManager.YafDBConnInfoMessageEventHandler( connMan_InfoMessage );
				// connMan.DBConnection.FireInfoMessageEventOnUserErrors = true;
				txtIndexStatistics.Text = DB.db_getstats_warning( connMan );
				DB.db_getstats( connMan );
			}
		}

		//Reindexing Database
		protected void btnReindex_Click( object sender, EventArgs e )
		{
			using ( YafDBConnManager connMan = new YafDBConnManager() )
			{
				connMan.InfoMessage += new YafDBConnManager.YafDBConnInfoMessageEventHandler( connMan_InfoMessage );
				txtIndexStatistics.Text = DB.db_reindex_warning( connMan );
				DB.db_reindex( connMan );
			}
		}

		//Mod By Touradg (herman_herman) 2009/10/19
		//Shrinking Database
		protected void btnShrink_Click( object sender, EventArgs e )
		{
			using ( YafDBConnManager DBName = new YafDBConnManager() )
				try
				{
					DBName.InfoMessage += new YafDBConnManager.YafDBConnInfoMessageEventHandler( connMan_InfoMessage );
					txtIndexStatistics.Text = DB.db_shrink_warning( DBName );
					DB.db_shrink( DBName );
					txtIndexStatistics.Text = "Shrink operation was Successful.Your database size is now: " + DB.DBSize + "MB";
				}
				catch ( Exception error )
				{
					txtIndexStatistics.Text = "Something went wrong with operation.The reported error is: " + error.Message;
				}
		}

		//Set Database Recovery Mode
		protected void btnRecoveryMode_Click( object sender, EventArgs e )
		{
			using ( YafDBConnManager DBName = new YafDBConnManager() )
			{
				try
				{
					String dbRecoveryMode = "";
					if ( RadioButtonList1.SelectedIndex == 0 )
					{
						dbRecoveryMode = "FULL";
					}
					if ( RadioButtonList1.SelectedIndex == 1 )
					{
						dbRecoveryMode = "SIMPLE";
					}
					if ( RadioButtonList1.SelectedIndex == 2 )
					{
						dbRecoveryMode = "BULK_LOGGED";
					}
					DBName.InfoMessage += new YafDBConnManager.YafDBConnInfoMessageEventHandler( connMan_InfoMessage );
					txtIndexStatistics.Text = DB.db_recovery_mode_warning( DBName );
					DB.db_recovery_mode( DBName, dbRecoveryMode );
					txtIndexStatistics.Text = "Database recovery mode was successfuly set to " + dbRecoveryMode;
				}
				catch ( Exception error )
				{
					txtIndexStatistics.Text = "Something went wrong with this operation.The reported error is: " + error.Message;
				}
			}
		}
		//End of MOD
		void connMan_InfoMessage( object sender, YafDBConnManager.YafDBConnInfoMessageEventArgs e )
		{
			txtIndexStatistics.Text = e.Message;
		}

	}
}
