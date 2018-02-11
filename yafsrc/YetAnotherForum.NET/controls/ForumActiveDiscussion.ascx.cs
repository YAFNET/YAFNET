/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The forum active discussion.
    /// </summary>
    public partial class ForumActiveDiscussion : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///  The last post tooltip string.
        /// </summary>
        private string lastPostToolTip;

        /// <summary>
        ///  The first Unread post tooltip string
        /// </summary>
        private string firstUnreadPostToolTip;

        #endregion

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
            var messageUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.posts, "m={0}#post{0}", currentRow["LastMessageID"]);

            // get the controls
            var newPostIcon = (Image)e.Item.FindControl("NewPostIcon");
            var textMessageLink = (HyperLink)e.Item.FindControl("TextMessageLink");
            var imageMessageLink = (HyperLink)e.Item.FindControl("ImageMessageLink");
            var lastPostedImage = (ThemeImage)e.Item.FindControl("LastPostedImage");
            var imageLastUnreadMessageLink = (HyperLink)e.Item.FindControl("ImageLastUnreadMessageLink");
            var lastUnreadImage = (ThemeImage)e.Item.FindControl("LastUnreadImage");
            var lastUserLink = (UserLink)e.Item.FindControl("LastUserLink");
            var lastPostedDateLabel = (DisplayDateTime)e.Item.FindControl("LastPostDate");
            var forumLink = (HyperLink)e.Item.FindControl("ForumLink");
            imageLastUnreadMessageLink.Visible = this.Get<YafBoardSettings>().ShowLastUnreadPost;

            // populate them...
            newPostIcon.AlternateText = this.GetText("NEW_POSTS");
            newPostIcon.ToolTip = this.GetText("NEW_POSTS");

            var topicSubject = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(currentRow["Topic"]));

            var styles = this.Get<YafBoardSettings>().UseStyledTopicTitles
                             ? this.Get<IStyleTransform>().DecodeStyleByString(currentRow["Styles"].ToString())
                             : string.Empty;

            if (styles.IsSet())
            {
                textMessageLink.Attributes.Add("style", styles);
            }

            if (currentRow["Status"].ToString().IsSet() && this.Get<YafBoardSettings>().EnableTopicStatus)
            {
                var topicStatusIcon = this.Get<ITheme>().GetItem("TOPIC_STATUS", currentRow["Status"].ToString());

                if (topicStatusIcon.IsSet() && !topicStatusIcon.Contains("[TOPIC_STATUS."))
                {
                    textMessageLink.Text =
                        @"<img src=""{0}"" alt=""{1}"" title=""{1}"" class=""topicStatusIcon"" />&nbsp;{2}"
                            .FormatWith(
                                this.Get<ITheme>().GetItem("TOPIC_STATUS", currentRow["Status"].ToString()),
                                this.GetText("TOPIC_STATUS", currentRow["Status"].ToString()),
                                topicSubject);
                }
                else
                {
                    textMessageLink.Text =
                        "[{0}]&nbsp;{1}".FormatWith(
                            this.GetText("TOPIC_STATUS", currentRow["Status"].ToString()), topicSubject);
                }
            }
            else
            {
                textMessageLink.Text = topicSubject;
            }

            textMessageLink.ToolTip =
                     "{0}".FormatWith(
                         this.GetTextFormatted(
                             "VIEW_TOPIC_STARTED_BY",
                             currentRow[this.Get<YafBoardSettings>().EnableDisplayName ? "UserDisplayName" : "UserName"]
                                 .ToString()));

            textMessageLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.posts, "t={0}&find=unread", currentRow["TopicID"]);

            imageMessageLink.NavigateUrl = messageUrl;
            lastPostedImage.LocalizedTitle = this.lastPostToolTip;

            if (imageLastUnreadMessageLink.Visible)
            {
                imageLastUnreadMessageLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.posts,
                    "t={0}&find=unread",
                    currentRow["TopicID"]);

                lastUnreadImage.LocalizedTitle = this.firstUnreadPostToolTip;
            }
            
            // Just in case...
            if (currentRow["LastUserID"] != DBNull.Value)
            {
                lastUserLink.UserID = currentRow["LastUserID"].ToType<int>();
                lastUserLink.Style = currentRow["LastUserStyle"].ToString();
                lastUserLink.ReplaceName = this.Get<YafBoardSettings>().EnableDisplayName
                              ? currentRow["LastUserDisplayName"].ToString()
                              : currentRow["LastUserName"].ToString();
            }

            if (currentRow["LastPosted"] != DBNull.Value)
            {
                lastPostedDateLabel.DateTime = currentRow["LastPosted"];

                var lastRead =
                    this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                        forumId: currentRow["ForumID"].ToType<int>(),
                        topicId: currentRow["TopicID"].ToType<int>(),
                        forumReadOverride: currentRow["LastForumAccess"].ToType<DateTime?>() ?? DateTimeHelper.SqlDbMinTime(),
                        topicReadOverride: currentRow["LastTopicAccess"].ToType<DateTime?>() ?? DateTimeHelper.SqlDbMinTime());

                if (DateTime.Parse(currentRow["LastPosted"].ToString()) > lastRead)
                {
                    this.Get<IYafSession>().UnreadTopics++;
                }

                lastUnreadImage.ThemeTag = (DateTime.Parse(currentRow["LastPosted"].ToString()) > lastRead)
                                               ? "ICON_NEWEST_UNREAD"
                                               : "ICON_LATEST_UNREAD";
                lastPostedImage.ThemeTag = (DateTime.Parse(currentRow["LastPosted"].ToString()) > lastRead)
                                               ? "ICON_NEWEST"
                                               : "ICON_LATEST";

                newPostIcon.ImageUrl = this.Get<ITheme>().GetItem(
                    "ICONS", (DateTime.Parse(currentRow["LastPosted"].ToString()) > lastRead) ? "TOPIC_NEW" : "TOPIC");
            }

            forumLink.Text = this.HtmlEncode(currentRow["Forum"].ToString());
            forumLink.ToolTip = this.GetText("COMMON", "VIEW_FORUM");
            forumLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.topics, "f={0}&name={1}", currentRow["ForumID"], currentRow["Forum"].ToString());
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
                this.Get<IYafSession>().UnreadTopics = 0;

                if (YafContext.Current.Settings.CategoryID > 0)
                {
                    activeTopics = LegacyDb.topic_latest_in_category(
                        boardID: this.PageContext.PageBoardID,
                        categoryID: YafContext.Current.Settings.CategoryID,
                        numOfPostsToRetrieve: this.Get<YafBoardSettings>().ActiveDiscussionsCount,
                        pageUserId: this.PageContext.PageUserID,
                        useStyledNicks: this.Get<YafBoardSettings>().UseStyledNicks,
                        showNoCountPosts: this.Get<YafBoardSettings>().NoCountForumsInActiveDiscussions,
                        findLastRead: this.Get<YafBoardSettings>().UseReadTrackingByDatabase);
                }
                else
                {
                    activeTopics = LegacyDb.topic_latest(
                         boardID: this.PageContext.PageBoardID,
                         numOfPostsToRetrieve: this.Get<YafBoardSettings>().ActiveDiscussionsCount,
                         pageUserId: this.PageContext.PageUserID,
                         useStyledNicks: this.Get<YafBoardSettings>().UseStyledNicks,
                         showNoCountPosts: this.Get<YafBoardSettings>().NoCountForumsInActiveDiscussions,
                         findLastRead: this.Get<YafBoardSettings>().UseReadTrackingByDatabase);
                }

                // Set colorOnly parameter to true, as we get all but color from css in the place
                if (this.Get<YafBoardSettings>().UseStyledNicks)
                {
                    this.Get<IStyleTransform>().DecodeStyleByTable(activeTopics, false, new[] { "LastUserStyle" });
                }

                if (this.PageContext.IsGuest)
                {
                    this.Get<IDataCache>().Set(
                        CacheKey,
                        activeTopics,
                        TimeSpan.FromMinutes(this.Get<YafBoardSettings>().ActiveDiscussionsCacheTimeout));
                }
            }

            this.CollapsibleImage.ToolTip = this.GetText("COMMON", "SHOWHIDE");

            this.RssFeed.Visible = this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().PostLatestFeedAccess);

            this.lastPostToolTip = this.GetText("DEFAULT", "GO_LAST_POST");
            this.firstUnreadPostToolTip = this.GetText("DEFAULT", "GO_LASTUNREAD_POST");
            this.LatestPosts.DataSource = activeTopics;
            this.LatestPosts.DataBind();
        }

        #endregion
    }
}