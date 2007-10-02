/* Yet Another Forum.NET
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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Controls
{
	public partial class ForumStatistics : YAF.Classes.Base.BaseUserControl
	{
		public ForumStatistics()
		{

		}

		public override void DataBind()
		{
			BindData();
			base.DataBind();
		}

		protected void BindData()
		{
			// Active users
			// Call this before forum_stats to clean up active users
			ActiveList.DataSource = YAF.Classes.Data.DB.active_list( PageContext.PageBoardID, null );

			// Forum statistics
			string key = YafCache.GetBoardCacheKey( Constants.Cache.BoardStats );
			DataRow stats = ( DataRow ) Cache [key];
			if ( stats == null )
			{
				stats = YAF.Classes.Data.DB.board_poststats( PageContext.PageBoardID );
				Cache.Insert( key, stats, null, DateTime.Now.AddMinutes( 15 ), TimeSpan.Zero );
			}

			Stats.Text = String.Format( PageContext.Localization.GetText( "stats_posts" ), stats ["posts"], stats ["topics"], stats ["forums"] );
			Stats.Text += "<br/>";

			if ( !stats.IsNull( "LastPost" ) )
			{
				Stats.Text += String.Format( PageContext.Localization.GetText( "stats_lastpost" ),
					YafDateTime.FormatDateTimeTopic( ( DateTime ) stats ["LastPost"] ),
					String.Format( "<a href=\"{0}\">{1}</a>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile, "u={0}", stats ["LastUserID"] ), HtmlEncode( stats ["LastUser"] ) )
				);
				Stats.Text += "<br/>";
			}

			Stats.Text += String.Format( PageContext.Localization.GetText( "stats_members" ), stats ["members"] );
			Stats.Text += "<br/>";

			Stats.Text += String.Format( PageContext.Localization.GetText( "stats_lastmember" ),
				String.Format( "<a href=\"{0}\">{1}</a>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile, "u={0}", stats ["LastMemberID"] ), HtmlEncode( stats ["LastMember"] ) )
			);
			Stats.Text += "<br/>";

			DataRow activeStats = YAF.Classes.Data.DB.active_stats( PageContext.PageBoardID );
			activeinfo.Text = String.Format( "<a href=\"{3}\">{0}</a> - {1}, {2}.",
				String.Format( PageContext.Localization.GetText( ( int ) activeStats ["ActiveUsers"] == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2" ), activeStats ["ActiveUsers"] ),
				String.Format( PageContext.Localization.GetText( ( int ) activeStats ["ActiveMembers"] == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2" ), activeStats ["ActiveMembers"] ),
				String.Format( PageContext.Localization.GetText( ( int ) activeStats ["ActiveGuests"] == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2" ), activeStats ["ActiveGuests"] ),
				YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.activeusers )
				);

			activeinfo.Text += "<br/>" + string.Format( PageContext.Localization.GetText( "MAX_ONLINE" ), PageContext.BoardSettings.MaxUsers, YafDateTime.FormatDateTimeTopic( PageContext.BoardSettings.MaxUsersWhen ) );

			UpdatePanel();
		}

		protected void expandInformation_Click( object sender, ImageClickEventArgs e )
		{
			// toggle the panel visability state...
			Mession.PanelState.TogglePanelState( "Information", PanelSessionState.CollapsiblePanelState.Expanded );
			if ( NeedDataBind != null ) NeedDataBind( this, new EventArgs() );
		}

		private void UpdatePanel()
		{
			expandInformation.ImageUrl = PageContext.Theme.GetCollapsiblePanelImageURL( "Information", PanelSessionState.CollapsiblePanelState.Expanded );
			InformationTBody.Visible = ( Mession.PanelState ["Information"] == PanelSessionState.CollapsiblePanelState.Expanded );
		}

		public event EventHandler<EventArgs> NeedDataBind;
	}
}