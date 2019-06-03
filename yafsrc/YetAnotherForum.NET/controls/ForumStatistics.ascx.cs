/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Text;
    using System.Threading;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
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
        /// Returns the formatted active users.
        /// </returns>
        [NotNull]
        protected string FormatActiveUsers([NotNull] DataRow activeStats)
        {
            var sb = new StringBuilder();

            var activeUsers = activeStats["ActiveUsers"].ToType<int>();
            var activeHidden = activeStats["ActiveHidden"].ToType<int>();
            var activeMembers = activeStats["ActiveMembers"].ToType<int>();
            var activeGuests = activeStats["ActiveGuests"].ToType<int>();

            // show hidden count to admin...
            if (this.PageContext.IsAdmin)
            {
                activeUsers += activeHidden;
            }

            var canViewActive = this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ActiveUsersViewPermissions);
            var showGuestTotal = (activeGuests > 0) &&
                                  (this.Get<YafBoardSettings>().ShowGuestsInDetailedActiveList ||
                                   this.Get<YafBoardSettings>().ShowCrawlersInActiveList);
            if (canViewActive &&
                (showGuestTotal || activeMembers > 0 && activeGuests <= 0))
            {
                // always show active users...       
                sb.Append(
                    string.Format(
                        "<a href=\"{1}\" title=\"{2}\">{0}</a>",
                        this.GetTextFormatted(
                                activeUsers == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2", activeUsers),
                            YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 0),
                            this.GetText("COMMON", "VIEW_FULLINFO"),
                            this.PageContext.IsCrawler ? " rel=\"nofolow\"" : string.Empty));
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
                        ? string.Format(
                            ", <a href=\"{1}\" title=\"{2}\"{3}>{0}</a>",
                            this.GetTextFormatted(
                                    activeMembers == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2", activeMembers),
                                YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 1),
                                this.GetText("COMMON", "VIEW_FULLINFO"),
                                this.PageContext.IsCrawler ? " rel=\"nofolow\"" : string.Empty)
                        : $", {this.GetTextFormatted(activeMembers == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2", activeMembers)}");
            }

            if (activeGuests > 0)
            {
                if (canViewActive &&
                    (this.Get<YafBoardSettings>().ShowGuestsInDetailedActiveList ||
                     this.Get<YafBoardSettings>().ShowCrawlersInActiveList))
                {
                    sb.Append(
                        string.Format(
                            ", <a href=\"{1}\" title=\"{2}\"{3}>{0}</a>",
                            this.GetTextFormatted(
                                    activeGuests == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2", activeGuests),
                                YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 2),
                                this.GetText("COMMON", "VIEW_FULLINFO"),
                                this.PageContext.IsCrawler ? " rel=\"nofolow\"" : string.Empty));
                }
                else
                {
                    sb.Append(
                        $", {this.GetTextFormatted(activeGuests == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2", activeGuests)}");
                }
            }

            if (activeHidden > 0 && this.PageContext.IsAdmin)
            {
                // vzrus: was temporary left as is, only admins can view hidden users online, why not everyone?
                if (activeHidden > 0 && this.PageContext.IsAdmin)
                {
                    sb.Append(
                      string.Format(
                        ", <a href=\"{1}\" title=\"{2}\">{0}</a>",
                        this.GetTextFormatted("ACTIVE_USERS_HIDDEN", activeHidden),
                            YafBuildLink.GetLink(ForumPages.activeusers, "v={0}", 3),
                            this.GetText("COMMON", "VIEW_FULLINFO")));
                }
                else
                {
                    sb.Append($", {this.GetTextFormatted("ACTIVE_USERS_HIDDEN", activeHidden)}");
                }
            }

            sb.Append($" {this.GetTextFormatted("ACTIVE_USERS_TIME", this.Get<YafBoardSettings>().ActiveListTime)}");

            return sb.ToString();
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
            var activeUsers = this.Get<IDataCache>().GetOrSet(
              Constants.Cache.UsersOnlineStatus,
              () => this.Get<YafDbBroker>().GetActiveList(false, this.Get<YafBoardSettings>().ShowCrawlersInActiveList),
              TimeSpan.FromMilliseconds(this.Get<YafBoardSettings>().OnlineStatusCacheTimeout));

            this.ActiveUsers1.ActiveUserTable = activeUsers;

            // "Active Users" Count and Most Users Count 
            var activeStats = this.GetRepository<Active>().Stats();

            this.ActiveUserCount.Text = this.FormatActiveUsers(activeStats);

            // Tommy MOD "Recent Users" Count.
            if (this.Get<YafBoardSettings>().ShowRecentUsers)
            {
                var activeUsers30Day = this.Get<IDataCache>().GetOrSet(
                    Constants.Cache.VisitorsInTheLast30Days,
                    () => this.Get<YafDbBroker>().GetRecentUsers(60 * 24 * 30),
                    TimeSpan.FromMinutes(this.Get<YafBoardSettings>().ForumStatisticsCacheTimeout));

                if (activeUsers30Day != null && activeUsers30Day.HasRows())
                {
                    activeUsers30Day.Locale = Thread.CurrentThread.CurrentCulture;

                    var activeUsers1Day1 =
                        activeUsers30Day.Select(
                            $"LastVisit >= '{DateTime.UtcNow.AddDays(-1).ToString(Thread.CurrentThread.CurrentCulture)}'");

                    this.RecentUsersCount.Text = this.GetTextFormatted(
                        "RECENT_ONLINE_USERS", activeUsers1Day1.Length, activeUsers30Day.Rows.Count);

                    if (activeUsers1Day1.Length > 0)
                    {
                        try
                        {
                            this.RecentUsers.ActiveUserTable = activeUsers1Day1.CopyToDataTable();
                            this.RecentUsers.Visible = true;
                        }
                        catch (Exception)
                        {
                            this.RecentUsers.Visible = false;
                        }
                    }

                    this.RecentUsersPlaceHolder.Visible = true;
                }
            }
            else
            {
                this.RecentUsersPlaceHolder.Visible = false;
            }

            // Forum Statistics
            var postsStatisticsDataRow = this.Get<IDataCache>().GetOrSet(
              Constants.Cache.BoardStats,
              () =>
              {
                  // get the post stats
                  var dr = this.GetRepository<Board>().Poststats(this.PageContext.PageBoardID, this.Get<YafBoardSettings>().UseStyledNicks, true);

                  // Set colorOnly parameter to false, as we get here color from data field in the place
                  dr["LastUserStyle"] = this.Get<YafBoardSettings>().UseStyledNicks
                                          ? this.Get<IStyleTransform>().DecodeStyleByString(
                                            dr["LastUserStyle"].ToString())
                                          : null;
                  return dr.Table;
              },
              TimeSpan.FromMinutes(this.Get<YafBoardSettings>().ForumStatisticsCacheTimeout)).Rows[0];

            // Forum Statistics
            var userStatisticsDataRow =
                this.Get<IDataCache>()
                    .GetOrSet(
                        Constants.Cache.BoardUserStats,
                        () => this.GetRepository<Board>().Userstats(this.PageContext.PageBoardID).Table,
                        TimeSpan.FromMinutes(this.Get<YafBoardSettings>().BoardUserStatsCacheTimeout))
                    .Rows[0];

            // show max users...
            if (!userStatisticsDataRow.IsNull("MaxUsers"))
            {
                this.MostUsersCount.Text = this.GetTextFormatted(
                  "MAX_ONLINE",
                  Convert.ToInt32(userStatisticsDataRow["MaxUsers"]),
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
                this.LastPostUserLink.ReplaceName = this.Get<YafBoardSettings>().EnableDisplayName
                                                        ? postsStatisticsDataRow["LastUserDisplayName"].ToString()
                                                        : postsStatisticsDataRow["LastUser"].ToString();
                this.LastPostUserLink.Style = postsStatisticsDataRow["LastUserStyle"].ToString();
                this.StatsLastPost.Text = this.GetTextFormatted(
                    "stats_lastpost",
                    new DisplayDateTime
                        {
                            DateTime = postsStatisticsDataRow["LastPost"],
                            Format = DateTimeFormat.BothTopic
                        }.RenderToString());
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
            this.NewestMemberUserLink.UserID = userStatisticsDataRow["LastMemberID"].ToType<int>();
            this.NewestMemberUserLink.ReplaceName = this.Get<YafBoardSettings>().EnableDisplayName
                                                        ? userStatisticsDataRow["LastMemberDisplayName"].ToString()
                                                        : userStatisticsDataRow["LastMember"].ToString();
        }

        #endregion
    }
}