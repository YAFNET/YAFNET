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
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

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
            var messageUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.posts, "m={0}#post{0}", currentRow["LastMessageID"]);

            // get the controls
            var postIcon = e.Item.FindControlAs<PlaceHolder>("PostIcon");
            var textMessageLink = (HyperLink)e.Item.FindControl("TextMessageLink");
            var imageMessageLink = e.Item.FindControlAs<ThemeButton>("GoToLastPost");
            var imageLastUnreadMessageLink = e.Item.FindControlAs<ThemeButton>("GoToLastUnread");
            var lastUserLink = (UserLink)e.Item.FindControl("LastUserLink");
            var lastPostedDateLabel = (DisplayDateTime)e.Item.FindControl("LastPostDate");
            var forumLink = (HyperLink)e.Item.FindControl("ForumLink");
            imageLastUnreadMessageLink.Visible = this.Get<YafBoardSettings>().ShowLastUnreadPost;

            var topicSubject = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(currentRow["Topic"]));

            var styles = this.Get<YafBoardSettings>().UseStyledTopicTitles
                             ? this.Get<IStyleTransform>().DecodeStyleByString(currentRow["Styles"].ToString())
                             : string.Empty;

            if (styles.IsSet())
            {
                textMessageLink.Attributes.Add("style", styles);
            }

            textMessageLink.Text = topicSubject;

            textMessageLink.ToolTip =
                $"{this.GetTextFormatted("VIEW_TOPIC_STARTED_BY", currentRow[this.Get<YafBoardSettings>().EnableDisplayName ? "UserDisplayName" : "UserName"].ToString())}";

            textMessageLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.posts, "t={0}&find=unread", currentRow["TopicID"]);

            imageMessageLink.NavigateUrl = messageUrl;

            if (imageLastUnreadMessageLink.Visible)
            {
                imageLastUnreadMessageLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.posts,
                    "t={0}&find=unread",
                    currentRow["TopicID"]);
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

                    var newMessage = e.Item.FindControlAs<Label>("NewMessage");
                    newMessage.Visible = true;
                }

                postIcon.Controls.Add(
                    new Literal
                    {
                        Text = 
                                $"<span class=\"fa-stack\"><i class=\"fas fa-comment fa-stack-2x {(DateTime.Parse(currentRow["LastPosted"].ToString()) > lastRead ? "text-success" : "text-secondary")}\"></i><i class=\"fas fa-comment fa-stack-1x fa-inverse\"></i></span>"
                    });
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
                    activeTopics = this.GetRepository<Topic>().LatestInCategoryAsDataTable(
                        boardId: this.PageContext.PageBoardID,
                        categoryId: YafContext.Current.Settings.CategoryID,
                        numOfPostsToRetrieve: this.Get<YafBoardSettings>().ActiveDiscussionsCount,
                        pageUserId: this.PageContext.PageUserID,
                        useStyledNicks: this.Get<YafBoardSettings>().UseStyledNicks,
                        showNoCountPosts: this.Get<YafBoardSettings>().NoCountForumsInActiveDiscussions,
                        findLastRead: this.Get<YafBoardSettings>().UseReadTrackingByDatabase);
                }
                else
                {
                    activeTopics = this.GetRepository<Topic>().LatestAsDataTable(
                        boardId: this.PageContext.PageBoardID,
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

            this.RssFeed.Visible = this.Footer.Visible =
                                       this.Get<IPermissions>()
                                           .Check(this.Get<YafBoardSettings>().PostLatestFeedAccess);

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