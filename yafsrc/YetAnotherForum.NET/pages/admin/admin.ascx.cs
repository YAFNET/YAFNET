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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for main.
	/// </summary>
	public partial class admin : YAF.Classes.Core.AdminPage
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", "" );

				// bind data
				BindBoardsList();
				BindData();
				//TODO UpgradeNotice.Visible = install._default.GetCurrentVersion() < Data.AppVersion;
			}
		}

		protected void Delete_Load( object sender, System.EventArgs e )
		{
			( ( LinkButton ) sender ).Attributes ["onclick"] = "return confirm('Delete this User?')";
		}

		protected void Approve_Load( object sender, System.EventArgs e )
		{
			( ( LinkButton ) sender ).Attributes ["onclick"] = "return confirm('Approve this User?')";
		}

		protected void DeleteAll_Load( object sender, System.EventArgs e )
		{
			( ( Button ) sender ).Attributes ["onclick"] = "return confirm('Delete all Unapproved Users more than 14 days old?')";
		}

		protected void ApproveAll_Load( object sender, System.EventArgs e )
		{
			( ( Button ) sender ).Attributes ["onclick"] = "return confirm('Approve all Users?')";
		}

		/// <summary>
		/// Bind list of boards to dropdown
		/// </summary>
		private void BindBoardsList()
		{
			// only if user is hostadmin, otherwise boards' list is hidden
			if ( PageContext.IsHostAdmin )
			{
				DataTable dt = YAF.Classes.Data.DB.board_list( null );

				// add row for "all boards" (null value)
				DataRow r = dt.NewRow();

				r ["BoardID"] = -1;
				r ["Name"] = " - All Boards -";

				dt.Rows.InsertAt( r, 0 );

				// set datasource
				BoardStatsSelect.DataSource = dt;
				BoardStatsSelect.DataBind();

				// select current board as default
				BoardStatsSelect.SelectedIndex =
					BoardStatsSelect.Items.IndexOf( BoardStatsSelect.Items.FindByValue( PageContext.PageBoardID.ToString() ) );
			}
		}


		/// <summary>
		/// Gets board ID for which to show statistics.
		/// </summary>
		/// <returns>Returns ID of selected board (for host admin), ID of current board (for admin), null if all boards is selected.</returns>
		private object GetSelectedBoardID()
		{
			// check dropdown only if user is hostadmin
			if ( PageContext.IsHostAdmin )
			{
				// -1 means all boards are selected
				if ( BoardStatsSelect.SelectedValue == "-1" )
					return null;
				else
					return BoardStatsSelect.SelectedValue;
			}
			// for non host admin user, return board he's logged on
			else return PageContext.PageBoardID;
		}

		private void BindData()
		{
			ActiveList.DataSource = YAF.Classes.Data.DB.active_list( PageContext.PageBoardID, true );
			UserList.DataSource = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, null, false );
			DataBind();

			// get stats for current board, selected board or all boards (see function)
			DataRow row = YAF.Classes.Data.DB.board_stats( GetSelectedBoardID() );

			NumPosts.Text = String.Format( "{0:N0}", row ["NumPosts"] );
			NumTopics.Text = String.Format( "{0:N0}", row ["NumTopics"] );
			NumUsers.Text = String.Format( "{0:N0}", row ["NumUsers"] );

			TimeSpan span = DateTime.Now - ( DateTime ) row ["BoardStart"];
			double days = span.Days;

			BoardStart.Text = String.Format( "{0:d} ({1:N0} days ago)", row ["BoardStart"], days );

			if ( days < 1 ) days = 1;
            DayPosts.Text = String.Format("{0:N2}", SqlDataLayerConverter.VerifyInt32(row["NumPosts"]) / days);
            DayTopics.Text = String.Format("{0:N2}", SqlDataLayerConverter.VerifyInt32(row["NumTopics"]) / days);
            DayUsers.Text = String.Format("{0:N2}", SqlDataLayerConverter.VerifyInt32(row["NumUsers"]) / days);

			DBSize.Text = String.Format( "{0} MB", DB.DBSize );
		}

		public void UserList_ItemCommand( object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e )
		{
			switch ( e.CommandName )
			{
				case "edit":
					YafBuildLink.Redirect( ForumPages.admin_edituser, "u={0}", e.CommandArgument );
					break;
				case "delete":
					UserMembershipHelper.DeleteUser( Convert.ToInt32( e.CommandArgument ) );
					BindData();
					break;
				case "approve":
					UserMembershipHelper.ApproveUser( Convert.ToInt32( e.CommandArgument ) );
					BindData();
					break;
				case "deleteall":
					UserMembershipHelper.DeleteAllUnapproved( DateTime.Now.AddDays( -14 ) );
					//YAF.Classes.Data.DB.user_deleteold( PageContext.PageBoardID );
					BindData();
					break;
				case "approveall":
					UserMembershipHelper.ApproveAll();
					//YAF.Classes.Data.DB.user_approveall( PageContext.PageBoardID );
					BindData();
					break;
			}
		}

		public void BoardStatsSelect_Changed( object sender, System.EventArgs e )
		{
			// re-bind data
			BindData();
		}

		protected string FormatForumLink( object ForumID, object ForumName )
		{
			if ( ForumID.ToString() == "" || ForumName.ToString() == "" )
				return "";

			return String.Format( "<a target=\"_top\" href=\"{0}\">{1}</a>", YafBuildLink.GetLink( ForumPages.topics, "f={0}", ForumID ), ForumName );
		}

		protected string FormatTopicLink( object TopicID, object TopicName )
		{
			if ( TopicID.ToString() == "" || TopicName.ToString() == "" )
				return "";

			return String.Format( "<a target=\"_top\" href=\"{0}\">{1}</a>", YafBuildLink.GetLink( ForumPages.posts, "t={0}", TopicID ), TopicName );
		}
	}
}
