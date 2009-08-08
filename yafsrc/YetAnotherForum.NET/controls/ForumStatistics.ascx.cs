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
using System.Data;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public partial class ForumStatistics : YAF.Classes.Core.BaseUserControl
	{
		public ForumStatistics()
		{
			this.Load += new EventHandler( ForumStatistics_Load );
		}

		void ForumStatistics_Load( object sender, EventArgs e )
		{
			// Active users
			// Call this before forum_stats to clean up active users
			ActiveUsers1.ActiveUserTable = YAF.Classes.Data.DB.active_list(PageContext.PageBoardID, null);

			// "Active Users" Count and Most Users Count
			DataRow activeStats = YAF.Classes.Data.DB.active_stats(PageContext.PageBoardID);

			ActiveUserCount.Text = FormatActiveUsers(activeStats);

			// Forum Statistics
			string key = YafCache.GetBoardCacheKey(Constants.Cache.BoardStats);
			DataRow statisticsDataRow = (DataRow)Cache[key];
			if (statisticsDataRow == null)
			{
				statisticsDataRow = YAF.Classes.Data.DB.board_poststats(PageContext.PageBoardID);
				Cache.Insert(key, statisticsDataRow, null, DateTime.Now.AddMinutes(PageContext.BoardSettings.ForumStatisticsCacheTimeout), TimeSpan.Zero);
			}

			// show max users...
			if (!statisticsDataRow.IsNull("MaxUsers"))
			{
				MostUsersCount.Text = PageContext.Localization.GetTextFormatted("MAX_ONLINE", statisticsDataRow["MaxUsers"], YafServices.DateTime.FormatDateTimeTopic(statisticsDataRow["MaxUsersWhen"]));
			}
			else
			{
				MostUsersCount.Text = PageContext.Localization.GetTextFormatted("MAX_ONLINE", activeStats["ActiveUsers"], YafServices.DateTime.FormatDateTimeTopic(DateTime.Now));
			}

			// Posts and Topic Count...
			StatsPostsTopicCount.Text = PageContext.Localization.GetTextFormatted("stats_posts", statisticsDataRow["posts"], statisticsDataRow["topics"], statisticsDataRow["forums"]);

			// Last post
			if (!statisticsDataRow.IsNull("LastPost"))
			{
				StatsLastPostHolder.Visible = true;

				LastPostUserLink.UserID = Convert.ToInt32(statisticsDataRow["LastUserID"]);
				LastPostUserLink.UserName = statisticsDataRow["LastUser"].ToString();

				StatsLastPost.Text = PageContext.Localization.GetTextFormatted("stats_lastpost", YafServices.DateTime.FormatDateTimeTopic((DateTime)statisticsDataRow["LastPost"]));
			}
			else
			{
				StatsLastPostHolder.Visible = false;
			}

			// Member Count
			StatsMembersCount.Text = PageContext.Localization.GetTextFormatted("stats_members", statisticsDataRow["members"]);

			// Newest Member
			StatsNewestMember.Text = PageContext.Localization.GetText("stats_lastmember");
			NewestMemberUserLink.UserID = Convert.ToInt32(statisticsDataRow["LastMemberID"]);
			NewestMemberUserLink.UserName = statisticsDataRow["LastMember"].ToString();
		}

		protected string FormatActiveUsers( DataRow activeStats )
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			int activeUsers = Convert.ToInt32( activeStats ["ActiveUsers"] );
			int activeHidden = Convert.ToInt32( activeStats ["ActiveHidden"] );
			int activeMembers = Convert.ToInt32( activeStats["ActiveMembers"]);
			int activeGuests = Convert.ToInt32( activeStats["ActiveGuests"]);

			// show hidden count to admin...
			if ( PageContext.IsAdmin ) activeUsers += activeHidden;

			if ( YafServices.Permissions.Check( PageContext.BoardSettings.ActiveUsersViewPermissions ) )
			{
				// always show active users...
				sb.Append( String.Format( "<a href=\"{1}\">{0}</a>",
					PageContext.Localization.GetTextFormatted( activeUsers == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2", activeUsers ),
					YafBuildLink.GetLink( ForumPages.activeusers ) ) );
			}
			else
			{
				// no link because no permissions...
				sb.Append( PageContext.Localization.GetTextFormatted( activeUsers == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2", activeUsers ) );
			}

			if ( activeMembers > 0 ) 
			{
				sb.Append( String.Format( ", {0}", PageContext.Localization.GetTextFormatted( activeMembers == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2", activeMembers ) ) );
			}

			if ( activeGuests > 0 ) 
			{
				sb.Append( String.Format( ", {0}", PageContext.Localization.GetTextFormatted( activeGuests == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2", activeGuests ) ) );
			}

			if ( activeHidden > 0 && PageContext.IsAdmin )
			{	
				sb.Append( String.Format( ", {0}", PageContext.Localization.GetTextFormatted( "ACTIVE_USERS_HIDDEN", activeHidden ) ) );
			}

			return sb.ToString();
		}
	}
}