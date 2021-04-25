/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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
    using System.Linq;
    using System.Text;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;

    using DateTime = System.DateTime;

    #endregion

    /// <summary>
    /// The forum user statistics.
    /// </summary>
    public partial class ForumStatsUsers : BaseUserControl
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ForumStatsUsers"/> class.
        /// </summary>
        public ForumStatsUsers()
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
        protected string FormatActiveUsers([NotNull] dynamic activeStats)
        {
            var sb = new StringBuilder();

            var activeUsers = (int)activeStats.ActiveUsers;
            var activeHidden = (int)activeStats.ActiveHidden;
            var activeMembers = (int)activeStats.ActiveMembers;
            var activeGuests = (int)activeStats.ActiveGuests;

            // show hidden count to admin...
            if (this.PageContext.IsAdmin)
            {
                activeUsers += activeHidden;
            }

            var canViewActive = this.Get<IPermissions>().Check(this.PageContext.BoardSettings.ActiveUsersViewPermissions);
            var showGuestTotal = activeGuests > 0 && (this.PageContext.BoardSettings.ShowGuestsInDetailedActiveList
                                                      || this.PageContext.BoardSettings.ShowCrawlersInActiveList);
            if (canViewActive && (showGuestTotal || activeMembers > 0 && activeGuests <= 0))
            {
                // always show active users...       
                sb.AppendFormat(
                    "<a href=\"{1}\" title=\"{2}\"{3}>{0}</a>",
                    this.GetTextFormatted(
                        activeUsers == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2",
                        activeUsers),
                    this.Get<LinkBuilder>().GetLink(ForumPages.ActiveUsers, "v={0}", 0),
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
                        ? $", <a href=\"{this.Get<LinkBuilder>().GetLink(ForumPages.ActiveUsers, "v={0}", 1)}\" title=\"{this.GetText("COMMON", "VIEW_FULLINFO")}\"{(this.PageContext.IsCrawler ? " rel=\"nofolow\"" : string.Empty)}>{this.GetTextFormatted(activeMembers == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2", activeMembers)}</a>"
                        : $", {this.GetTextFormatted(activeMembers == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2", activeMembers)}");
            }

            if (activeGuests > 0)
            {
                if (canViewActive && (this.PageContext.BoardSettings.ShowGuestsInDetailedActiveList
                                      || this.PageContext.BoardSettings.ShowCrawlersInActiveList))
                {
                    sb.AppendFormat(
                        ", <a href=\"{1}\" title=\"{2}\"{3}>{0}</a>",
                        this.GetTextFormatted(
                            activeGuests == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2",
                            activeGuests),
                        this.Get<LinkBuilder>().GetLink(ForumPages.ActiveUsers, "v={0}", 2),
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
                    this.Get<LinkBuilder>().GetLink(ForumPages.ActiveUsers, "v={0}", 3),
                    this.GetText("COMMON", "VIEW_FULLINFO"));
            }

            sb.Append($" {this.GetTextFormatted("ACTIVE_USERS_TIME", this.PageContext.BoardSettings.ActiveListTime)}");

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
                () => this.GetRepository<Active>().List(
                    false,
                    this.PageContext.BoardSettings.ShowCrawlersInActiveList,
                    this.PageContext.BoardSettings.ActiveListTime),
                TimeSpan.FromMilliseconds(this.PageContext.BoardSettings.OnlineStatusCacheTimeout));

            this.ActiveUsers1.ActiveUsersList = activeUsers;

            // "Active Users" Count and Most Users Count 
            var activeStats = this.GetRepository<Active>().Stats(this.PageContext.PageBoardID);

            this.ActiveUserCount.Text = this.FormatActiveUsers(activeStats);

            // Tommy MOD "Recent Users" Count.
            if (this.PageContext.BoardSettings.ShowRecentUsers)
            {
                var activeUsers30Day = this.Get<IDataCache>().GetOrSet(
                     Constants.Cache.VisitorsInTheLast30Days,
                     () => this.GetRepository<User>().GetRecentUsers(
                         60 * 24 * 30,
                         this.PageContext.BoardSettings.UseStyledNicks),
                     TimeSpan.FromMinutes(this.PageContext.BoardSettings.ForumStatisticsCacheTimeout));

                if (activeUsers30Day != null && activeUsers30Day.Any())
                {
                    var activeUsers1Day1 = activeUsers30Day.Where(x => x.LastVisit >= DateTime.UtcNow.AddDays(-1)).ToList();

                    this.RecentUsersCount.Text = this.GetTextFormatted(
                        "RECENT_ONLINE_USERS",
                        activeUsers1Day1.Count,
                        activeUsers30Day.Count);

                    if (activeUsers1Day1.Any())
                    {
                        try
                        {
                            this.RecentUsers.ActiveUsersList = activeUsers1Day1;
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

            // show max users...
            if (this.PageContext.BoardSettings.MaxUsers > 0)
            {
                this.MostUsersCount.Text = this.GetTextFormatted(
                    "MAX_ONLINE",
                    this.PageContext.BoardSettings.MaxUsers,
                    this.Get<IDateTimeService>().FormatDateTimeTopic(this.PageContext.BoardSettings.MaxUsersWhen));
            }
            else
            {
                this.MostUsersCount.Text = this.GetTextFormatted(
                    "MAX_ONLINE",
                    (int)activeStats.ActiveUsers,
                    this.Get<IDateTimeService>().FormatDateTimeTopic(DateTime.UtcNow));
            }
        }

        #endregion
    }
}