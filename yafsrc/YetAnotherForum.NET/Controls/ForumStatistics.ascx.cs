/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

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

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;

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

            var canViewActive = this.Get<IPermissions>().Check(this.Get<BoardSettings>().ActiveUsersViewPermissions);
            var showGuestTotal = activeGuests > 0 && (this.Get<BoardSettings>().ShowGuestsInDetailedActiveList
                                                      || this.Get<BoardSettings>().ShowCrawlersInActiveList);
            if (canViewActive && (showGuestTotal || activeMembers > 0 && activeGuests <= 0))
            {
                // always show active users...       
                sb.AppendFormat(
                    "<a href=\"{1}\" title=\"{2}\"{3}>{0}</a>",
                    this.GetTextFormatted(
                        activeUsers == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2",
                        activeUsers),
                    BuildLink.GetLink(ForumPages.ActiveUsers, "v={0}", 0),
                    this.GetText("COMMON", "VIEW_FULLINFO"),
                    this.PageContext.IsCrawler ? " rel=\"nofolow\"" : string.Empty);
            }
            else
            {
                // no link because no permissions...
                sb.Append(
                    this.GetTextFormatted(
                        activeUsers == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2",
                        activeUsers));
            }

            if (activeMembers > 0)
            {
                sb.Append(
                    canViewActive
                        ? $", <a href=\"{BuildLink.GetLink(ForumPages.ActiveUsers, "v={0}", 1)}\" title=\"{this.GetText("COMMON", "VIEW_FULLINFO")}\"{(this.PageContext.IsCrawler ? " rel=\"nofolow\"" : string.Empty)}>{this.GetTextFormatted(activeMembers == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2", activeMembers)}</a>"
                        : $", {this.GetTextFormatted(activeMembers == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2", activeMembers)}");
            }

            if (activeGuests > 0)
            {
                if (canViewActive && (this.Get<BoardSettings>().ShowGuestsInDetailedActiveList
                                      || this.Get<BoardSettings>().ShowCrawlersInActiveList))
                {
                    sb.AppendFormat(
                        ", <a href=\"{1}\" title=\"{2}\"{3}>{0}</a>",
                        this.GetTextFormatted(
                            activeGuests == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2",
                            activeGuests),
                        BuildLink.GetLink(ForumPages.ActiveUsers, "v={0}", 2),
                        this.GetText("COMMON", "VIEW_FULLINFO"),
                        this.PageContext.IsCrawler ? " rel=\"nofolow\"" : string.Empty);
                }
                else
                {
                    sb.Append(
                        $", {this.GetTextFormatted(activeGuests == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2", activeGuests)}");
                }
            }

            if (activeHidden > 0 && this.PageContext.IsAdmin)
            {
                sb.AppendFormat(
                    ", <a href=\"{1}\" title=\"{2}\">{0}</a>",
                    this.GetTextFormatted("ACTIVE_USERS_HIDDEN", activeHidden),
                    BuildLink.GetLink(ForumPages.ActiveUsers, "v={0}", 3),
                    this.GetText("COMMON", "VIEW_FULLINFO"));
            }

            sb.Append($" {this.GetTextFormatted("ACTIVE_USERS_TIME", this.Get<BoardSettings>().ActiveListTime)}");

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
            // Forum Statistics
            var postsStatisticsDataRow = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.BoardStats,
                () =>
                    {
                        // get the post stats
                        var dr = this.GetRepository<Board>().PostStats(
                            this.PageContext.PageBoardID,
                            this.Get<BoardSettings>().UseStyledNicks,
                            true);

                        // Set colorOnly parameter to false, as we get here color from data field in the place
                        dr["LastUserStyle"] = this.Get<BoardSettings>().UseStyledNicks
                                                  ? this.Get<IStyleTransform>().DecodeStyleByString(
                                                      dr["LastUserStyle"].ToString())
                                                  : null;
                        return dr.Table;
                    },
                TimeSpan.FromMinutes(this.Get<BoardSettings>().ForumStatisticsCacheTimeout)).Rows[0];

            // Forum Statistics
            var userStatisticsDataRow = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.BoardUserStats,
                () => this.GetRepository<Board>().UserStats(this.PageContext.PageBoardID).Table,
                TimeSpan.FromMinutes(this.Get<BoardSettings>().BoardUserStatsCacheTimeout)).Rows[0];

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
                this.LastPostUserLink.ReplaceName = this.Get<BoardSettings>().EnableDisplayName
                                                        ? postsStatisticsDataRow["LastUserDisplayName"].ToString()
                                                        : postsStatisticsDataRow["LastUser"].ToString();
                this.LastPostUserLink.Style = postsStatisticsDataRow["LastUserStyle"].ToString();
                this.StatsLastPost.Text = this.GetTextFormatted(
                    "stats_lastpost",
                    new DisplayDateTime
                        {
                            DateTime = postsStatisticsDataRow["LastPost"], Format = DateTimeFormat.BothTopic
                        }.RenderToString());
            }
            else
            {
                this.StatsLastPostHolder.Visible = false;
            }

            // Member Count
            this.StatsMembersCount.Text = this.GetTextFormatted("stats_members", userStatisticsDataRow["members"]);

            // Newest Member
            this.StatsNewestMember.Text = this.GetText("stats_lastmember");
            this.NewestMemberUserLink.UserID = userStatisticsDataRow["LastMemberID"].ToType<int>();
            this.NewestMemberUserLink.ReplaceName = this.Get<BoardSettings>().EnableDisplayName
                                                        ? userStatisticsDataRow["LastMemberDisplayName"].ToString()
                                                        : userStatisticsDataRow["LastMember"].ToString();

            if (this.Get<BoardSettings>().DeniedRegistrations > 0 || this.Get<BoardSettings>().BannedUsers > 0
                                                                     || this.Get<BoardSettings>().ReportedSpammers
                                                                     > 0)
            {
                this.AntiSpamStatsHolder.Visible = true;
                this.StatsSpamDenied.Param0 = this.Get<BoardSettings>().DeniedRegistrations.ToString();
                this.StatsSpamBanned.Param0 = this.Get<BoardSettings>().BannedUsers.ToString();
                this.StatsSpamReported.Param0 = this.Get<BoardSettings>().ReportedSpammers.ToString();
            }
            else
            {
                this.AntiSpamStatsHolder.Visible = false;
            }
        }

        #endregion
    }
}