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
    using System.Globalization;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Utilities;
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
    /// The forum active discussion.
    /// </summary>
    public partial class ForumActiveDiscussion : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// The latest posts_ item data bound.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void LatestPosts_ItemDataBound([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
        {
            // populate the controls here...
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var currentRow = (DataRowView)e.Item.DataItem;

            // make message url...
            var messageUrl = BuildLink.GetLinkNotEscaped(
                ForumPages.Posts, "m={0}#post{0}", currentRow["LastMessageID"]);

            // get the controls
            var postIcon = e.Item.FindControlAs<Label>("PostIcon");
            var textMessageLink = e.Item.FindControlAs<HyperLink>("TextMessageLink");
            var info = e.Item.FindControlAs<ThemeButton>("Info");
            var imageMessageLink = e.Item.FindControlAs<ThemeButton>("GoToLastPost");
            var imageLastUnreadMessageLink = e.Item.FindControlAs<ThemeButton>("GoToLastUnread");
            var lastUserLink = new UserLink();
            var lastPostedDateLabel = new DisplayDateTime { Format = DateTimeFormat.BothTopic };

            var topicSubject = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(currentRow["Topic"]));

            var styles = this.Get<BoardSettings>().UseStyledTopicTitles
                             ? this.Get<IStyleTransform>().DecodeStyleByString(currentRow["Styles"].ToString())
                             : string.Empty;

            if (styles.IsSet())
            {
                textMessageLink.Attributes.Add("style", styles);
            }

            textMessageLink.Text = topicSubject;

            var startedByText = this.GetTextFormatted(
                "VIEW_TOPIC_STARTED_BY",
                currentRow[this.Get<BoardSettings>().EnableDisplayName ? "UserDisplayName" : "UserName"].ToString());

            var inForumText = this.GetTextFormatted("IN_FORUM", this.HtmlEncode(currentRow["Forum"].ToString()));

            textMessageLink.ToolTip =
                $"{startedByText} {inForumText}";
            textMessageLink.Attributes.Add("data-toggle", "tooltip");

            textMessageLink.NavigateUrl = BuildLink.GetLinkNotEscaped(
                ForumPages.Posts, "t={0}&find=unread", currentRow["TopicID"]);

            imageMessageLink.NavigateUrl = messageUrl;

            if (imageLastUnreadMessageLink.Visible)
            {
                imageLastUnreadMessageLink.NavigateUrl = BuildLink.GetLinkNotEscaped(
                    ForumPages.Posts,
                    "t={0}&find=unread",
                    currentRow["TopicID"]);
            }
            
            // Just in case...
            if (currentRow["LastUserID"] != DBNull.Value)
            {
                lastUserLink.UserID = currentRow["LastUserID"].ToType<int>();
                lastUserLink.Style = currentRow["LastUserStyle"].ToString();
                lastUserLink.ReplaceName = this.Get<BoardSettings>().EnableDisplayName
                              ? currentRow["LastUserDisplayName"].ToString()
                              : currentRow["LastUserName"].ToString();
            }

            if (currentRow["LastPosted"] != DBNull.Value)
            {
                lastPostedDateLabel.DateTime = currentRow["LastPosted"];

                var lastRead =
                    this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                        currentRow["ForumID"].ToType<int>(),
                        currentRow["TopicID"].ToType<int>(),
                        currentRow["LastForumAccess"].ToType<DateTime?>() ?? DateTimeHelper.SqlDbMinTime(),
                        currentRow["LastTopicAccess"].ToType<DateTime?>() ?? DateTimeHelper.SqlDbMinTime());

                if (DateTime.Parse(currentRow["LastPosted"].ToString()) > lastRead)
                {
                    postIcon.CssClass = "badge badge-success";

                    postIcon.Text = this.GetText("NEW_MESSAGE");
                }
            }

            var lastPostedDateTime = currentRow["LastPosted"].ToType<DateTime>();

            var formattedDatetime = this.Get<BoardSettings>().ShowRelativeTime
                                        ? lastPostedDateTime.ToString(
                                            "yyyy-MM-ddTHH:mm:ssZ",
                                            CultureInfo.InvariantCulture)
                                        : this.Get<IDateTime>().Format(
                                            DateTimeFormat.BothTopic,
                                            lastPostedDateTime);

            var span = this.Get<BoardSettings>().ShowRelativeTime ? @"<span class=""popover-timeago"">" : "<span>";

            info.TextLocalizedTag = "by";
            info.TextLocalizedPage = "DEFAULT";
            info.ParamText0 = this.Get<BoardSettings>().EnableDisplayName
                                  ? currentRow["LastUserDisplayName"].ToString()
                                  : currentRow["LastUserName"].ToString();
            
            info.DataContent = $@"
                          {lastUserLink.RenderToString()}
                          <span class=""fa-stack"">
                                                    <i class=""fa fa-calendar-day fa-stack-1x text-secondary""></i>
                                                    <i class=""fa fa-circle fa-badge-bg fa-inverse fa-outline-inverse""></i>
                                                    <i class=""fa fa-clock fa-badge text-secondary""></i>
                                                </span>&nbsp;{span}{formattedDatetime}</span>
                         ";
        }

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            this.PageContext.PageElements.RegisterJsBlockStartup(
                "TopicLinkPopoverJs",
                JavaScriptBlocks.TopicLinkPopoverJs(
                    $"{this.GetText("LASTPOST")}&nbsp;{this.GetText("SEARCH", "BY")} ...",
                    ".topic-link-popover",
                    "focus hover"));

            base.OnPreRender(e);
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Latest forum posts
            // Shows the latest n number of posts on the main forum list page
            const string CacheKey = Constants.Cache.ForumActiveDiscussions;

            DataTable activeTopics = null;

            if (this.PageContext.IsGuest)
            {
                // allow caching since this is a guest...
                activeTopics = this.Get<IDataCache>()[CacheKey] as DataTable;
            }

            if (activeTopics == null)
            {
                this.Get<ISession>().UnreadTopics = 0;

                if (BoardContext.Current.Settings.CategoryID > 0)
                {
                    activeTopics = this.GetRepository<Topic>().LatestInCategoryAsDataTable(
                        this.PageContext.PageBoardID,
                        BoardContext.Current.Settings.CategoryID,
                        this.Get<BoardSettings>().ActiveDiscussionsCount,
                        this.PageContext.PageUserID,
                        this.Get<BoardSettings>().UseStyledNicks,
                        this.Get<BoardSettings>().NoCountForumsInActiveDiscussions,
                        this.Get<BoardSettings>().UseReadTrackingByDatabase);
                }
                else
                {
                    activeTopics = this.GetRepository<Topic>().LatestAsDataTable(
                        this.PageContext.PageBoardID,
                        this.Get<BoardSettings>().ActiveDiscussionsCount,
                        this.PageContext.PageUserID,
                        this.Get<BoardSettings>().UseStyledNicks,
                        this.Get<BoardSettings>().NoCountForumsInActiveDiscussions,
                        this.Get<BoardSettings>().UseReadTrackingByDatabase);
                }

                // Set colorOnly parameter to true, as we get all but color from css in the place
                if (this.Get<BoardSettings>().UseStyledNicks)
                {
                    this.Get<IStyleTransform>().DecodeStyleByTable(activeTopics, false, new[] { "LastUserStyle" });
                }

                if (this.PageContext.IsGuest)
                {
                    this.Get<IDataCache>().Set(
                        CacheKey,
                        activeTopics,
                        TimeSpan.FromMinutes(this.Get<BoardSettings>().ActiveDiscussionsCacheTimeout));
                }
            }

            this.RssFeed.Visible = this.Footer.Visible =
                                       this.Get<IPermissions>()
                                           .Check(this.Get<BoardSettings>().PostLatestFeedAccess);

            if (!this.Get<BoardSettings>().ShowRSSLink && !this.Get<BoardSettings>().ShowAtomLink)
            {
                this.Footer.Visible = false;
            }

            this.LatestPosts.DataSource = activeTopics;
            this.LatestPosts.DataBind();

            if (activeTopics.Rows.Count == 0)
            {
                this.ActiveDiscussionPlaceHolder.Visible = false;
            }
        }

        #endregion
    }
}