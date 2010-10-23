/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;

namespace YAF.Controls
{
    using System;
    using System.Data;
    using System.Text;
    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Data;
    using YAF.Classes.Utils;

    /// <summary>
    /// The forum statistics.
    /// </summary>
    public partial class ForumStatistics : BaseUserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForumStatistics"/> class.
        /// </summary>
        public ForumStatistics()
        {
            Load += this.ForumStatistics_Load;
        }

        /// <summary>
        /// The forum statistics_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ForumStatistics_Load(object sender, EventArgs e)
        {
            // Active users : Call this before forum_stats to clean up active users
            string key = YafCache.GetBoardCacheKey(Constants.Cache.UsersOnlineStatus);
            DataTable activeUsers = PageContext.Cache.GetItem(
                key, (double) YafContext.Current.BoardSettings.OnlineStatusCacheTimeout,
                () =>
                this.Get<YafDBBroker>().GetActiveList(false, YafContext.Current.BoardSettings.ShowCrawlersInActiveList));

            this.ActiveUsers1.ActiveUserTable = activeUsers;

            // "Active Users" Count and Most Users Count
            DataRow activeStats = DB.active_stats(PageContext.PageBoardID);
            this.ActiveUserCount.Text = this.FormatActiveUsers(activeStats);

            // Forum Statistics
            key = YafCache.GetBoardCacheKey(Constants.Cache.BoardStats);
            var postsStatisticsDataRow = PageContext.Cache.GetItem(
                key,
                PageContext.BoardSettings.ForumStatisticsCacheTimeout,
                () =>
                    {
                        // get the post stats
                        DataRow dr = DB.board_poststats(PageContext.PageBoardID,
                                                        PageContext.BoardSettings.UseStyledNicks,
                                                        true);

                        // Set colorOnly parameter to false, as we get here color from data field in the place
                        dr["LastUserStyle"] = this.PageContext.BoardSettings.UseStyledNicks
                                                  ? new StyleTransform(PageContext.Theme).DecodeStyleByString(
                                                      dr["LastUserStyle"].ToString(), false)
                                                  : null;
                        return dr;
                    });

            // Forum Statistics

            var userStatisticsDataRow = PageContext.Cache.GetItem(
                YafCache.GetBoardCacheKey(Constants.Cache.BoardUserStats),
                PageContext.BoardSettings.BoardUserStatsCacheTimeout,
                () => DB.board_userstats(PageContext.PageBoardID));

            // show max users...
            if (!userStatisticsDataRow.IsNull("MaxUsers"))
            {
                this.MostUsersCount.Text = PageContext.Localization.GetTextFormatted(
                    "MAX_ONLINE", userStatisticsDataRow["MaxUsers"],
                    this.Get<YafDateTime>().FormatDateTimeTopic(userStatisticsDataRow["MaxUsersWhen"]));
            }
            else
            {
                this.MostUsersCount.Text = PageContext.Localization.GetTextFormatted(
                    "MAX_ONLINE", activeStats["ActiveUsers"],
                    this.Get<YafDateTime>().FormatDateTimeTopic(DateTime.UtcNow));
            }

            // Posts and Topic Count...
            this.StatsPostsTopicCount.Text = PageContext.Localization.GetTextFormatted(
                "stats_posts", postsStatisticsDataRow["posts"], postsStatisticsDataRow["topics"],
                postsStatisticsDataRow["forums"]);

            // Last post
            if (!postsStatisticsDataRow.IsNull("LastPost"))
            {
                this.StatsLastPostHolder.Visible = true;

                this.LastPostUserLink.UserID = Convert.ToInt32(postsStatisticsDataRow["LastUserID"]);
                this.LastPostUserLink.Style = postsStatisticsDataRow["LastUserStyle"].ToString();
                this.StatsLastPost.Text = PageContext.Localization.GetTextFormatted(
                    "stats_lastpost",
                    this.Get<YafDateTime>().FormatDateTimeTopic((DateTime) postsStatisticsDataRow["LastPost"]));
            }
            else
            {
                this.StatsLastPostHolder.Visible = false;
            }

            // Member Count
            this.StatsMembersCount.Text = PageContext.Localization.GetTextFormatted("stats_members",
                                                                                    userStatisticsDataRow["members"]);

            // Newest Member
            this.StatsNewestMember.Text = PageContext.Localization.GetText("stats_lastmember");
            this.NewestMemberUserLink.UserID = Convert.ToInt32(userStatisticsDataRow["LastMemberID"]);

            // Todays Birthdays
            StatsTodaysBirthdays.Text = PageContext.Localization.GetText("stats_birthdays");// "";

            // get users for this board...
            List<DataRow> users = DB.user_list(PageContext.PageBoardID, null, null).Rows.Cast<DataRow>().ToList();

            foreach (var BirthdayUserLink in from user in users
                                             let userProfile = YafUserProfile.GetProfile(UserMembershipHelper.GetUserNameFromID((int) user["UserID"]))
                                             where userProfile.Birthday > DateTime.MinValue
                                             let today = DateTime.Today
                                             where today.Month.Equals(userProfile.Birthday.Month) && today.Day.Equals(userProfile.Birthday.Day)
                                             select new UserLink
                                                        {
                                                            UserID = (int) user["UserID"], PostfixText = " ({0})".FormatWith(GetUserAge(userProfile.Birthday))
                                                        })
            {
                BirthdayUsers.Controls.Add(BirthdayUserLink);

                var Separator = new HtmlGenericControl { InnerHtml = "&nbsp;,&nbsp;" };

                BirthdayUsers.Controls.Add(Separator);
                    
                BirthdayUsers.Visible = true;
            }

            if (BirthdayUsers.Visible)
            {
                // Remove last Separator
                BirthdayUsers.Controls.RemoveAt(BirthdayUsers.Controls.Count - 1);
            }
        }

        /// <summary>
        /// Get the Users age
        /// </summary>
        /// <param name="Birthdate">Birthdate of the User</param>
        /// <returns>The Age</returns>
        private static int GetUserAge(DateTime Birthdate)
        {
            var userAge = DateTime.Now.Year - Birthdate.Year;

            if (DateTime.Now < Birthdate.AddYears(userAge))
            {
                userAge--;
            }
            return userAge;
        }

        /// <summary>
        /// The format active users.
        /// </summary>
        /// <param name="activeStats">
        /// The active stats.
        /// </param>
        /// <returns>
        /// The format active users.
        /// </returns>
        protected string FormatActiveUsers(DataRow activeStats)
        {
            var sb = new StringBuilder();

            int activeUsers = Convert.ToInt32(activeStats["ActiveUsers"]);
            int activeHidden = Convert.ToInt32(activeStats["ActiveHidden"]);
            int activeMembers = Convert.ToInt32(activeStats["ActiveMembers"]);
            int activeGuests = Convert.ToInt32(activeStats["ActiveGuests"]);

            // show hidden count to admin...
            if (PageContext.IsAdmin)
            {
                activeUsers += activeHidden;
            }

            bool canViewActive = this.Get<YafPermissions>().Check(PageContext.BoardSettings.ActiveUsersViewPermissions);
            bool showGuestTotal = (activeGuests > 0) && (PageContext.BoardSettings.ShowGuestsInDetailedActiveList || PageContext.BoardSettings.ShowCrawlersInActiveList);
            bool showActiveHidden = (activeHidden > 0) && PageContext.IsAdmin;
            if (canViewActive && ((showGuestTotal) || (activeMembers > 0 && (showGuestTotal || activeGuests <= 0)) || (showActiveHidden && (activeMembers > 0) && showGuestTotal)))
            {

                // always show active users...       
                sb.Append(
                  "<a href=\"{1}\">{0}</a>".FormatWith(this.PageContext.Localization.GetTextFormatted(activeUsers == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2", activeUsers), YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 0)));
            }
            else
            {
                // no link because no permissions...
                sb.Append(PageContext.Localization.GetTextFormatted(activeUsers == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2", activeUsers));
            }

            if (activeMembers > 0)
            {
                sb.Append(
                    canViewActive
                        ? ", <a href=\"{1}\">{0}</a>".FormatWith(
                            this.PageContext.Localization.GetTextFormatted(
                                activeMembers == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2", activeMembers),
                            YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 1))
                        : ", {0}".FormatWith(
                            this.PageContext.Localization.GetTextFormatted(
                                activeMembers == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2", activeMembers)));
            }

            if (activeGuests > 0)
            {
                if (canViewActive && (PageContext.BoardSettings.ShowGuestsInDetailedActiveList || PageContext.BoardSettings.ShowCrawlersInActiveList))
                {
                    sb.Append(
                    ", <a href=\"{1}\">{0}</a>".FormatWith(this.PageContext.Localization.GetTextFormatted(activeGuests == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2", activeGuests), YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 2)));
                }
                else
                {
                    sb.Append(
                      ", {0}".FormatWith(this.PageContext.Localization.GetTextFormatted(activeGuests == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2", activeGuests)));
                }
            }

            if (activeHidden > 0 && PageContext.IsAdmin)
            {
                // vzrus: was temporary left as is, only admins can view hidden users online, why not everyone?
                if (activeHidden > 0 && PageContext.IsAdmin)
                {
                    sb.Append(
                                ", <a href=\"{1}\">{0}</a>".FormatWith(this.PageContext.Localization.GetTextFormatted("ACTIVE_USERS_HIDDEN", activeHidden), YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 3)));
                }
                else
                {
                    sb.Append(", {0}".FormatWith(this.PageContext.Localization.GetTextFormatted("ACTIVE_USERS_HIDDEN", activeHidden)));
                }
            }

            sb.Append(" {0}".FormatWith(this.PageContext.Localization.GetTextFormatted("ACTIVE_USERS_TIME", this.PageContext.BoardSettings.ActiveListTime)));

            return sb.ToString();
        }
    }
}