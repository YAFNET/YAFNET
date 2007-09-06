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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages
{
	/// <summary>
	/// Summary description for _default.
	/// </summary>
	public partial class forum : YAF.Classes.Base.ForumPage
	{

		public forum()
			: base( "DEFAULT" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				if (PageContext.IsPrivate && User==null)
				{
					YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.login, "ReturnUrl={0}", Request.RawUrl );
				}

				TimeNow.Text = String.Format( GetText( "CURRENT_TIME" ), YafDateTime.FormatTime( DateTime.Now ) );
				TimeLastVisit.Text = String.Format( GetText( "last_visit" ), YafDateTime.FormatDateTime( Mession.LastVisit ) );
				MarkAll.Text = GetText( "MARKALL" );

				if ( PageContext.UnreadPrivate > 0 )
				{
					UnreadMsgs.Visible = true;
					UnreadMsgs.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_pm );
					if ( PageContext.UnreadPrivate == 1 )
						UnreadMsgs.Text = String.Format( GetText( "unread1" ), PageContext.UnreadPrivate );
					else
						UnreadMsgs.Text = String.Format( GetText( "unread0" ), PageContext.UnreadPrivate );
				}

				if ( PageContext.Settings.LockedForum == 0 )
				{
					PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
					if ( PageContext.PageCategoryID != 0 )
					{
						PageLinks.AddLink( PageContext.PageCategoryName, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum, "c={0}", PageContext.PageCategoryID ) );
						Welcome.Visible = false;
					}
				}

				BindData();
			}

		}

		protected void MarkAll_Click( object sender, System.EventArgs e )
		{
			Mession.LastVisit = DateTime.Now;
			BindData();
		}

		private void BindData()
		{
			DataSet ds = YAF.Classes.Data.DB.board_layout( PageContext.PageBoardID, PageContext.PageUserID, PageContext.PageCategoryID, null );
			CategoryList.DataSource = ds.Tables ["yaf_Category"];

			// Active users
			// Call this before forum_stats to clean up active users
			ActiveList.DataSource = YAF.Classes.Data.DB.active_list( PageContext.PageBoardID, null );

			// Latest forum posts
			// Shows the latest n number of posts on the main forum list page
			LatestPosts.DataSource = YAF.Classes.Data.DB.topic_latest( PageContext.PageBoardID, 7, PageContext.PageUserID );

			// Forum statistics
			string key = YafCache.GetBoardCacheKey(Constants.Cache.BoardStats);
			DataRow stats = (DataRow)Cache[key];
			if ( stats == null )
			{
				stats = YAF.Classes.Data.DB.board_poststats( PageContext.PageBoardID );
				Cache.Insert( key, stats, null, DateTime.Now.AddMinutes( 15 ), TimeSpan.Zero );
			}

			Stats.Text = String.Format( GetText( "stats_posts" ), stats ["posts"], stats ["topics"], stats ["forums"] );
			Stats.Text += "<br/>";

			if ( !stats.IsNull( "LastPost" ) )
			{
				Stats.Text += String.Format( GetText( "stats_lastpost" ),
					YafDateTime.FormatDateTimeTopic( ( DateTime ) stats ["LastPost"] ),
					String.Format( "<a href=\"{0}\">{1}</a>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile, "u={0}", stats ["LastUserID"] ), Server.HtmlEncode( stats ["LastUser"].ToString() ) )
				);
				Stats.Text += "<br/>";
			}

			Stats.Text += String.Format( GetText( "stats_members" ), stats ["members"] );
			Stats.Text += "<br/>";

			Stats.Text += String.Format( GetText( "stats_lastmember" ),
				String.Format( "<a href=\"{0}\">{1}</a>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile, "u={0}", stats ["LastMemberID"] ), Server.HtmlEncode( stats ["LastMember"].ToString() ) )
				);
			Stats.Text += "<br/>";

			DataRow activeStats = YAF.Classes.Data.DB.active_stats( PageContext.PageBoardID );
			activeinfo.Text = String.Format( "<a href=\"{3}\">{0}</a> - {1}, {2}.",
				String.Format( GetText( ( int ) activeStats ["ActiveUsers"] == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2" ), activeStats ["ActiveUsers"] ),
				String.Format( GetText( ( int ) activeStats ["ActiveMembers"] == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2" ), activeStats ["ActiveMembers"] ),
				String.Format( GetText( ( int ) activeStats ["ActiveGuests"] == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2" ), activeStats ["ActiveGuests"] ),
				YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.activeusers )
				);

			activeinfo.Text += "<br/>" + string.Format( GetText( "MAX_ONLINE" ), PageContext.BoardSettings.MaxUsers, YafDateTime.FormatDateTimeTopic( PageContext.BoardSettings.MaxUsersWhen ) );

			UpdateActiveDiscussionsPanel();
			UpdateInformationPanel();

			DataBind();
		}

		protected string FormatLastPost( System.Data.DataRow row )
		{
			if ( !row.IsNull( "LastPosted" ) )
			{
				string minipost;
				if ( DateTime.Parse( row ["LastPosted"].ToString() ) > Mession.LastVisit )
					minipost = GetThemeContents( "ICONS", "ICON_NEWEST" );
				else
					minipost = GetThemeContents( "ICONS", "ICON_LATEST" );

				return String.Format( "{0}<br/>{1}<br/>{2}&nbsp;<a title=\"{4}\" href=\"{5}\"><img src='{3}'></a>",
					YafDateTime.FormatDateTimeTopic( Convert.ToDateTime( row ["LastPosted"] ) ),
					String.Format( GetText( "in" ), String.Format( "<a href=\"{0}\">{1}</a>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "t={0}", row ["LastTopicID"] ), row ["LastTopicName"] ) ),
					String.Format( GetText( "by" ), String.Format( "<a href=\"{0}\">{1}</a>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile, "u={0}", row ["LastUserID"] ), row ["LastUser"] ) ),
					minipost,
					GetText( "GO_LAST_POST" ),
					YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "m={0}#{0}", row ["LastMessageID"] )
					);
			}
			else
				return GetText( "NO_POSTS" );
		}

		protected string GetViewing( object o )
		{
			DataRow row = ( DataRow ) o;
			int nViewing = ( int ) row ["Viewing"];
			if ( nViewing > 0 )
				return "&nbsp;" + String.Format( GetText( "VIEWING" ), nViewing );
			else
				return "";
		}

		protected string GetForumIcon( object o )
		{
			DataRow row = ( DataRow ) o;
			bool locked = ( bool ) row ["Locked"];
			DateTime lastRead = Mession.GetForumRead( ( int ) row ["ForumID"] );
			DateTime lastPosted = row ["LastPosted"] != DBNull.Value ? ( DateTime ) row ["LastPosted"] : lastRead;

			string img, imgTitle;

			try
			{
				if ( locked )
				{
					img = GetThemeContents( "ICONS", "FORUM_LOCKED" );
					imgTitle = GetText( "ICONLEGEND", "Forum_Locked" );
				}
				else if ( lastPosted > lastRead )
				{
					img = GetThemeContents( "ICONS", "FORUM_NEW" );
					imgTitle = GetText( "ICONLEGEND", "New_Posts" );
				}
				else
				{
					img = GetThemeContents( "ICONS", "FORUM" );
					imgTitle = GetText( "ICONLEGEND", "No_New_Posts" );
				}
			}
			catch ( Exception )
			{
				img = GetThemeContents( "ICONS", "FORUM" );
				imgTitle = GetText( "ICONLEGEND", "No_New_Posts" );
			}

			return String.Format( "<img src=\"{0}\" title=\"{1}\"/>", img, imgTitle );
		}

		protected void ModeratorList_ItemCommand( object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e )
		{
			//PageContext.AddLoadMessage("TODO: Fix this");
			//TODO: Show moderators
		}

		private void UpdateActiveDiscussionsPanel()
		{
			expandActiveDiscussions.ImageUrl = GetCollapsiblePanelImageURL( "ActiveDiscussions", PanelSessionState.CollapsiblePanelState.Expanded );
			ActiveDiscussionTBody.Visible = ( Mession.PanelState ["ActiveDiscussions"] == PanelSessionState.CollapsiblePanelState.Expanded );
		}

		private void UpdateInformationPanel()
		{
			expandInformation.ImageUrl = GetCollapsiblePanelImageURL( "Information", PanelSessionState.CollapsiblePanelState.Expanded );
			InformationTBody.Visible = ( Mession.PanelState ["Information"] == PanelSessionState.CollapsiblePanelState.Expanded );
		}

		protected void expandActiveDiscussions_Click( object sender, ImageClickEventArgs e )
		{
			// toggle the panel visability state...
			Mession.PanelState.TogglePanelState( "ActiveDiscussions", PanelSessionState.CollapsiblePanelState.Expanded );
			BindData();
		}

		protected void expandInformation_Click( object sender, ImageClickEventArgs e )
		{
			// toggle the panel visability state...
			Mession.PanelState.TogglePanelState( "Information", PanelSessionState.CollapsiblePanelState.Expanded );
			BindData();
		}

		protected void categoryList_ItemCommand( object source, RepeaterCommandEventArgs e )
		{
			if ( (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) && e.CommandName == "panel" )
			{
				ImageButton tmpImage = ( ImageButton ) e.Item.FindControl( "expandCategory" );
				YAF.Controls.ForumList tmpForumList = ( YAF.Controls.ForumList ) e.Item.FindControl( "forumList" );

				Mession.PanelState.TogglePanelState( "categoryPanel" + e.CommandArgument, PanelSessionState.CollapsiblePanelState.Expanded );

				BindData();
			}
		}

		protected void CategoryList_ItemDataBound( object sender, RepeaterItemEventArgs e )
		{
			if ( ( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem ) )
			{
				ImageButton tmpImage = ( ImageButton ) e.Item.FindControl( "expandCategory" );
				YAF.Controls.ForumList tmpForumList = ( YAF.Controls.ForumList ) e.Item.FindControl( "forumList" );

				tmpImage.ImageUrl = GetCollapsiblePanelImageURL( "categoryPanel" + tmpImage.CommandArgument, PanelSessionState.CollapsiblePanelState.Expanded );
				tmpForumList.Visible = tmpForumList.Visible = ( Mession.PanelState ["categoryPanel" + tmpImage.CommandArgument] == PanelSessionState.CollapsiblePanelState.Expanded );
			}
		}
	}
}
