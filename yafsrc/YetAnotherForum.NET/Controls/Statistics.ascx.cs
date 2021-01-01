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

    using YAF.Core.BaseControls;
    using YAF.Core.Model;
    using YAF.Core.Utilities.Helpers;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Web.Controls;

    #endregion

    /// <summary>
    /// The forum statistics.
    /// </summary>
    public partial class Statistics : BaseUserControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Statistics" /> class.
        /// </summary>
        public Statistics()
        {
            this.Load += this.ForumStatistics_Load;
        }

        #endregion

        #region Methods

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
            var postsStatistics = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.BoardStats,
                () => this.GetRepository<Board>().PostStats(this.PageContext.PageBoardID, true),
                TimeSpan.FromMinutes(this.PageContext.BoardSettings.ForumStatisticsCacheTimeout));

            var latestUser = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.BoardUserStats,
                () => this.GetRepository<User>().Latest(this.PageContext.PageBoardID),
                TimeSpan.FromMinutes(this.PageContext.BoardSettings.BoardUserStatsCacheTimeout));

            // Posts and Topic Count...
            this.StatsPostsTopicCount.Text = this.GetTextFormatted(
                "stats_posts",
                (int)postsStatistics.Posts,
                (int)postsStatistics.Topics,
                (int)postsStatistics.Forums);

            // Last post
            if (postsStatistics.LastPost != null)
            {
                this.StatsLastPostHolder.Visible = true;

                this.LastPostUserLink.UserID = postsStatistics.LastUserID;
                this.LastPostUserLink.ReplaceName = this.PageContext.BoardSettings.EnableDisplayName
                    ? postsStatistics.LastUserDisplayName
                    : postsStatistics.LastUser;
                this.LastPostUserLink.Suspended = (DateTime?)postsStatistics.LastUserSuspended;
                this.LastPostUserLink.Style = postsStatistics.LastUserStyle;
                this.StatsLastPost.Text = this.GetTextFormatted(
                    "stats_lastpost",
                    new DisplayDateTime
                        {
                            DateTime = (DateTime)postsStatistics.LastPost, Format = DateTimeFormat.BothTopic
                        }.RenderToString());
            }
            else
            {
                this.StatsLastPostHolder.Visible = false;
            }

            var membersCount = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.BoardMembers,
                () => this.GetRepository<User>().BoardMembers(this.PageContext.PageBoardID),
                TimeSpan.FromMinutes(this.PageContext.BoardSettings.BoardUserStatsCacheTimeout));

            // Member Count
            this.StatsMembersCount.Text = this.GetTextFormatted("stats_members", membersCount);

            // Newest Member
            this.StatsNewestMember.Text = this.GetText("stats_lastmember");
            this.NewestMemberUserLink.UserID = latestUser.ID;
            this.NewestMemberUserLink.ReplaceName = this.PageContext.BoardSettings.EnableDisplayName
                ? latestUser.DisplayName
                : latestUser.Name;
            this.NewestMemberUserLink.Style = latestUser.UserStyle;
            this.NewestMemberUserLink.Suspended = latestUser.Suspended;

            if (this.PageContext.BoardSettings.DeniedRegistrations > 0 || this.PageContext.BoardSettings.BannedUsers > 0
                                                                     || this.PageContext.BoardSettings.ReportedSpammers
                                                                     > 0)
            {
                this.AntiSpamStatsHolder.Visible = true;
                this.StatsSpamDenied.Param0 = this.PageContext.BoardSettings.DeniedRegistrations.ToString();
                this.StatsSpamBanned.Param0 = this.PageContext.BoardSettings.BannedUsers.ToString();
                this.StatsSpamReported.Param0 = this.PageContext.BoardSettings.ReportedSpammers.ToString();
            }
            else
            {
                this.AntiSpamStatsHolder.Visible = false;
            }
        }

        #endregion
    }
}