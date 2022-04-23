/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

using YAF.Types.Models;

namespace YAF.Controls
{
    #region Using

    using System.Globalization;

    using YAF.Web.Controls;

    #endregion

    /// <summary>
    /// The active discussion.
    /// </summary>
    public partial class ActiveDiscussion : BaseUserControl
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

            var item = (LatestTopic)e.Item.DataItem;

            var topicSubject = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(item.Topic));

            // get the controls
            var postIcon = e.Item.FindControlAs<Label>("PostIcon");
            var textMessageLink = e.Item.FindControlAs<ThemeButton>("TextMessageLink");
            var forumLink = e.Item.FindControlAs<ThemeButton>("ForumLink");
            var info = e.Item.FindControlAs<ThemeButton>("Info");
            var lastUserLink = new UserLink();
            var lastPostedDateLabel = new DisplayDateTime { Format = DateTimeFormat.BothTopic };

            var topicStyle = item.Styles;

            var styles = this.PageBoardContext.BoardSettings.UseStyledTopicTitles && topicStyle.IsSet()
                ? this.Get<IStyleTransform>().Decode(item.Styles)
                : string.Empty;

            if (styles.IsSet())
            {
                textMessageLink.Attributes.Add("style", styles);
            }

            textMessageLink.Text = topicSubject;

            var startedByText = this.GetTextFormatted(
                "VIEW_TOPIC_STARTED_BY",
                this.PageBoardContext.BoardSettings.EnableDisplayName ? item.UserDisplayName : item.UserName);

            var inForumText = this.GetTextFormatted("IN_FORUM", this.HtmlEncode(item.Forum));

            textMessageLink.TitleNonLocalized = $"{startedByText} {inForumText}";
            textMessageLink.NavigateUrl = this.Get<LinkBuilder>().GetLink(
                ForumPages.Posts,
                new { t = item.TopicID, name = topicSubject });

            forumLink.Text = $"({item.Forum})";
            forumLink.NavigateUrl = this.Get<LinkBuilder>().GetForumLink(item.ForumID, item.Forum);

            lastUserLink.UserID = item.LastUserID.Value;
            lastUserLink.Style = item.LastUserStyle;
            lastUserLink.Suspended = item.LastUserSuspended;
            lastUserLink.ReplaceName = this.PageBoardContext.BoardSettings.EnableDisplayName
                ? item.LastUserDisplayName
                : item.LastUserName;

            lastPostedDateLabel.DateTime = item.LastPosted;

            var lastRead = this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                item.ForumID,
                item.TopicID,
                item.LastForumAccess ?? DateTimeHelper.SqlDbMinTime(),
                item.LastTopicAccess ?? DateTimeHelper.SqlDbMinTime());

            if ((DateTime)item.LastPosted > lastRead)
            {
                postIcon.Visible = true;
                postIcon.CssClass = "badge bg-success";

                postIcon.Text = this.GetText("NEW_MESSAGE");
            }

            var lastPostedDateTime = (DateTime)item.LastPosted;

            var formattedDatetime = this.PageBoardContext.BoardSettings.ShowRelativeTime
                                        ? lastPostedDateTime.ToRelativeTime()
                                        : this.Get<IDateTimeService>().Format(
                                            DateTimeFormat.BothTopic,
                                            lastPostedDateTime);

            info.TextLocalizedTag = "by";
            info.TextLocalizedPage = "DEFAULT";
            info.ParamText0 = this.PageBoardContext.BoardSettings.EnableDisplayName
                                  ? item.LastUserDisplayName
                                  : item.LastUserName;

            info.DataContent = $@"
                          {lastUserLink.RenderToString()}
                          <span class=""fa-stack"">
                                                    <i class=""fa fa-calendar-day fa-stack-1x text-secondary""></i>
                                                    <i class=""fa fa-circle fa-badge-bg fa-inverse fa-outline-inverse""></i>
                                                    <i class=""fa fa-clock fa-badge text-secondary""></i>
                                                </span>&nbsp;{formattedDatetime}
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
            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
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
            const string CacheKey = Constants.Cache.ActiveDiscussions;

            List<LatestTopic> activeTopics = null;

            if (this.PageBoardContext.IsGuest)
            {
                // allow caching since this is a guest...
                activeTopics = this.Get<IDataCache>()[CacheKey] as List<LatestTopic>;
            }

            if (activeTopics == null)
            {
                this.Get<ISession>().UnreadTopics = 0;

                activeTopics = this.GetRepository<Topic>().Latest(
                    this.PageBoardContext.PageBoardID,
                    this.PageBoardContext.PageCategoryID,
                    this.PageBoardContext.BoardSettings.ActiveDiscussionsCount,
                    this.PageBoardContext.PageUserID,
                    this.PageBoardContext.BoardSettings.NoCountForumsInActiveDiscussions,
                    this.PageBoardContext.BoardSettings.UseReadTrackingByDatabase);

                if (this.PageBoardContext.IsGuest)
                {
                    this.Get<IDataCache>().Set(
                        CacheKey,
                        activeTopics,
                        TimeSpan.FromMinutes(this.PageBoardContext.BoardSettings.ActiveDiscussionsCacheTimeout));
                }
            }

            this.RssFeed.Visible = this.Footer.Visible =
                                       this.Get<IPermissions>()
                                           .Check(this.PageBoardContext.BoardSettings.PostLatestFeedAccess);

            if (!this.PageBoardContext.BoardSettings.ShowAtomLink)
            {
                this.Footer.Visible = false;
            }

            this.LatestPosts.DataSource = activeTopics;
            this.LatestPosts.DataBind();

            if (!activeTopics.Any())
            {
                this.ActiveDiscussionPlaceHolder.Visible = false;
            }
        }

        #endregion
    }
}