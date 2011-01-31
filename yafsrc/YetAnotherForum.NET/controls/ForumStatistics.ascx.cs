/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;
  using System.Text;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// The forum statistics.
  /// </summary>
  public partial class ForumStatistics : BaseUserControl
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ForumStatistics" /> class.
    /// </summary>
    public ForumStatistics()
    {
      this.Load += this.ForumStatistics_Load;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The format active users.
    /// </summary>
    /// <param name="activeStats">
    /// The active stats.
    /// </param>
    /// <returns>
    /// The format active users.
    /// </returns>
    [NotNull]
    protected string FormatActiveUsers([NotNull] DataRow activeStats)
    {
      var sb = new StringBuilder();

      int activeUsers = Convert.ToInt32(activeStats["ActiveUsers"]);
      int activeHidden = Convert.ToInt32(activeStats["ActiveHidden"]);
      int activeMembers = Convert.ToInt32(activeStats["ActiveMembers"]);
      int activeGuests = Convert.ToInt32(activeStats["ActiveGuests"]);

      // show hidden count to admin...
      if (this.PageContext.IsAdmin)
      {
        activeUsers += activeHidden;
      }

      bool canViewActive = this.Get<IPermissions>().Check(this.PageContext.BoardSettings.ActiveUsersViewPermissions);
      bool showGuestTotal = (activeGuests > 0) &&
                            (this.PageContext.BoardSettings.ShowGuestsInDetailedActiveList ||
                             this.PageContext.BoardSettings.ShowCrawlersInActiveList);
      bool showActiveHidden = (activeHidden > 0) && this.PageContext.IsAdmin;
      if (canViewActive &&
          (showGuestTotal || (activeMembers > 0 && (showGuestTotal || activeGuests <= 0)) ||
           (showActiveHidden && (activeMembers > 0) && showGuestTotal)))
      {
        // always show active users...       
        sb.Append(
          "<a href=\"{1}\" alt=\"{2}\" title=\"{2}\" >{0}</a>".FormatWith(
            this.GetTextFormatted(
              activeUsers == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2", activeUsers), 
            YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 0), 
            this.GetText("COMMON", "VIEW_FULLINFO")));
      }
      else
      {
        // no link because no permissions...
        sb.Append(
          this.GetTextFormatted(
            activeUsers == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2", activeUsers));
      }

      if (activeMembers > 0)
      {
        sb.Append(
          canViewActive
            ? ", <a href=\"{1}\" alt=\"{2}\" title=\"{2}\" >{0}</a>".FormatWith(
              this.GetTextFormatted(
                activeMembers == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2", activeMembers), 
              YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 1), 
              this.GetText("COMMON", "VIEW_FULLINFO"))
            : ", {0}".FormatWith(
              this.GetTextFormatted(
                activeMembers == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2", activeMembers)));
      }

      if (activeGuests > 0)
      {
        if (canViewActive &&
            (this.PageContext.BoardSettings.ShowGuestsInDetailedActiveList ||
             this.PageContext.BoardSettings.ShowCrawlersInActiveList))
        {
          sb.Append(
            ", <a href=\"{1}\" alt=\"{2}\" title=\"{2}\" >{0}</a>".FormatWith(
              this.GetTextFormatted(
                activeGuests == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2", activeGuests), 
              YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 2), 
              this.GetText("COMMON", "VIEW_FULLINFO")));
        }
        else
        {
          sb.Append(
            ", {0}".FormatWith(
              this.GetTextFormatted(
                activeGuests == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2", activeGuests)));
        }
      }

      if (activeHidden > 0 && this.PageContext.IsAdmin)
      {
        // vzrus: was temporary left as is, only admins can view hidden users online, why not everyone?
        if (activeHidden > 0 && this.PageContext.IsAdmin)
        {
          sb.Append(
            ", <a href=\"{1}\" alt=\"{2}\" title=\"{2}\">{0}</a>".FormatWith(
              this.GetTextFormatted("ACTIVE_USERS_HIDDEN", activeHidden), 
              YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 3), 
              this.GetText("COMMON", "VIEW_FULLINFO")));
        }
        else
        {
          sb.Append(
            ", {0}".FormatWith(this.GetTextFormatted("ACTIVE_USERS_HIDDEN", activeHidden)));
        }
      }

      sb.Append(
        " {0}".FormatWith(
          this.GetTextFormatted(
            "ACTIVE_USERS_TIME", this.PageContext.BoardSettings.ActiveListTime)));

      return sb.ToString();
    }

    /// <summary>
    /// Get the Users age
    /// </summary>
    /// <param name="Birthdate">
    /// Birthdate of the User
    /// </param>
    /// <returns>
    /// The Age
    /// </returns>
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
    /// The forum statistics_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumStatistics_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      // Active users : Call this before forum_stats to clean up active users
      DataTable activeUsers = this.Get<IDataCache>().GetOrSet(
        Constants.Cache.UsersOnlineStatus,
        () => this.Get<IDBBroker>().GetActiveList(false, YafContext.Current.BoardSettings.ShowCrawlersInActiveList),
        TimeSpan.FromMilliseconds(YafContext.Current.BoardSettings.OnlineStatusCacheTimeout));

      this.ActiveUsers1.ActiveUserTable = activeUsers;

      // "Active Users" Count and Most Users Count
      DataRow activeStats = LegacyDb.active_stats(this.PageContext.PageBoardID);
      this.ActiveUserCount.Text = this.FormatActiveUsers(activeStats);

      // Forum Statistics
      var postsStatisticsDataRow = this.Get<IDataCache>().GetOrSet(
        Constants.Cache.BoardStats,
        () =>
          {
            // get the post stats
            DataRow dr = LegacyDb.board_poststats(
              this.PageContext.PageBoardID, this.PageContext.BoardSettings.UseStyledNicks, true);

            // Set colorOnly parameter to false, as we get here color from data field in the place
            dr["LastUserStyle"] = this.PageContext.BoardSettings.UseStyledNicks
                                    ? this.Get<IStyleTransform>().DecodeStyleByString(
                                      dr["LastUserStyle"].ToString(), false)
                                    : null;
            return dr;
          },
        TimeSpan.FromMinutes(this.PageContext.BoardSettings.ForumStatisticsCacheTimeout));

      // Forum Statistics
      var userStatisticsDataRow = this.Get<IDataCache>().GetOrSet(
        Constants.Cache.BoardUserStats,
        () => LegacyDb.board_userstats(this.PageContext.PageBoardID),
        TimeSpan.FromMinutes(this.PageContext.BoardSettings.BoardUserStatsCacheTimeout));

      // show max users...
      if (!userStatisticsDataRow.IsNull("MaxUsers"))
      {
        this.MostUsersCount.Text = this.GetTextFormatted(
          "MAX_ONLINE", 
          userStatisticsDataRow["MaxUsers"], 
          this.Get<IDateTime>().FormatDateTimeTopic(userStatisticsDataRow["MaxUsersWhen"]));
      }
      else
      {
        this.MostUsersCount.Text = this.GetTextFormatted(
          "MAX_ONLINE", activeStats["ActiveUsers"], this.Get<IDateTime>().FormatDateTimeTopic(DateTime.UtcNow));
      }

      // Posts and Topic Count...
      this.StatsPostsTopicCount.Text = this.GetTextFormatted(
        "stats_posts", 
        postsStatisticsDataRow["posts"], 
        postsStatisticsDataRow["topics"], 
        postsStatisticsDataRow["forums"]);

      // Last post
      if (!postsStatisticsDataRow.IsNull("LastPost"))
      {
        this.StatsLastPostHolder.Visible = true;

        this.LastPostUserLink.UserID = postsStatisticsDataRow["LastUserID"].ToType<int>();
        this.LastPostUserLink.Style = postsStatisticsDataRow["LastUserStyle"].ToString();
        this.StatsLastPost.Text = this.GetTextFormatted(
          "stats_lastpost", 
          new DisplayDateTime { DateTime = postsStatisticsDataRow["LastPost"], Format = DateTimeFormat.BothTopic }.
            RenderToString());
      }
      else
      {
        this.StatsLastPostHolder.Visible = false;
      }

      // Member Count
      this.StatsMembersCount.Text = this.GetTextFormatted(
        "stats_members", userStatisticsDataRow["members"]);

      // Newest Member
      this.StatsNewestMember.Text = this.GetText("stats_lastmember");
      this.NewestMemberUserLink.UserID = Convert.ToInt32(userStatisticsDataRow["LastMemberID"]);

      // Todays Birthdays
      // tha_watcha : Disabled as future feature, until its cached?!
      /*StatsTodaysBirthdays.Text = this.GetText("stats_birthdays");// "";

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
            }*/
    }

    #endregion
  }
}