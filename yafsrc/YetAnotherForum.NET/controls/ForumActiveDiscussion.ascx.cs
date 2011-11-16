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
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

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
            string messageUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.posts, "m={0}&find=lastpost", currentRow["LastMessageID"]);

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

            var topicSubject = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(currentRow["TOPIC"]));

            if (currentRow["Status"].ToString().IsSet() && this.Get<YafBoardSettings>().EnableTopicStatus)
            {
                var topicStatusIcon = this.Get<ITheme>().GetItem("TOPIC_STATUS", currentRow["Status"].ToString());

                if (topicStatusIcon.IsSet() &&
                    !topicStatusIcon.Contains("[TOPIC_STATUS."))
                {
                    textMessageLink.Text =
                    "<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" style=\"border: 0;width:16px;height:16px\" />&nbsp;{2}".FormatWith(
                    this.Get<ITheme>().GetItem("TOPIC_STATUS", currentRow["Status"].ToString()),
                        this.GetText("TOPIC_STATUS", currentRow["Status"].ToString()),
                        topicSubject);
                }
                else
                {
                    textMessageLink.Text =
                    "[{0}]&nbsp;{1}".FormatWith(
                        this.GetText("TOPIC_STATUS", currentRow["Status"].ToString()),
                        topicSubject);
                }
            }
            else
            {
                textMessageLink.Text = topicSubject;
            }

            if (!this.PageContext.IsMobileDevice)
            {
                textMessageLink.ToolTip = this.GetText("COMMON", "VIEW_TOPIC");
                textMessageLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.posts, "t={0}", currentRow["TopicID"]);
            }
            else
            {
                textMessageLink.ToolTip = this.GetText("DEFAULT", "GO_LASTUNREAD_POST");
                textMessageLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.posts, "t={0}&find=unread", currentRow["TopicID"]);
            }

            imageMessageLink.NavigateUrl = messageUrl;
            lastPostedImage.LocalizedTitle = this.lastPostToolTip;

            if (imageLastUnreadMessageLink.Visible)
            {
                imageLastUnreadMessageLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    //// ForumPages.posts, "m={0}&find=unread", currentRow["LastMessageID"]);
                    ForumPages.posts, "t={0}&find=unread", currentRow["TopicID"]);

                lastUnreadImage.LocalizedTitle = this.firstUnreadPostToolTip;
            }

            // Just in case...
            if (currentRow["LastUserID"] != DBNull.Value)
            {
                lastUserLink.UserID = currentRow["LastUserID"].ToType<int>();
                lastUserLink.Style = currentRow["LastUserStyle"].ToString();
            }

            if (currentRow["LastPosted"] != DBNull.Value)
            {
                lastPostedDateLabel.DateTime = currentRow["LastPosted"];
                DateTime lastRead;
                DateTime lastReadForum;

                if (this.Get<YafBoardSettings>().UseReadTrackingByDatabase)
                {
                    try
                    {
                        lastRead = currentRow["LastTopicAccess"] != DBNull.Value
                                       ? currentRow["LastTopicAccess"].ToType<DateTime>()
                                       : DateTime.MinValue.AddYears(1902);
                    }
                    catch (Exception)
                    {
                        lastRead = this.Get<IReadTracking>().GetTopicRead(
                            this.PageContext.PageUserID, currentRow["LastTopicID"].ToType<int>());
                    }

                    try
                    {
                        lastReadForum = currentRow["LastForumAccess"] != DBNull.Value
                                       ? currentRow["LastForumAccess"].ToType<DateTime>()
                                       : DateTime.MinValue.AddYears(1902);
                    }
                    catch (Exception)
                    {
                        lastReadForum = this.Get<IReadTracking>().GetForumRead(
                            this.PageContext.PageUserID, currentRow["ForumID"].ToType<int>());
                    }
                }
                else
                {
                   lastRead = this.Get<IYafSession>().GetTopicRead(currentRow["TopicID"].ToType<int>());
                   lastReadForum = this.Get<IYafSession>().GetForumRead(currentRow["ForumID"].ToType<int>());
                }

                if (lastReadForum > lastRead)
                {
                    lastRead = lastReadForum;
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

            forumLink.Text = this.Page.HtmlEncode(currentRow["Forum"].ToString());
            forumLink.ToolTip = this.GetText("COMMON", "VIEW_FORUM");
            forumLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.topics, "f={0}", currentRow["ForumID"]);
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
            	activeTopics = this.Get<IDbFunction>().GetData.topic_latest(
            		this.PageContext.PageBoardID,
            		this.Get<YafBoardSettings>().ActiveDiscussionsCount,
            		this.PageContext.PageUserID,
            		this.Get<YafBoardSettings>().UseStyledNicks,
            		this.Get<YafBoardSettings>().NoCountForumsInActiveDiscussions,
            		this.Get<YafBoardSettings>().UseReadTrackingByDatabase);

                // Set colorOnly parameter to true, as we get all but color from css in the place
                if (this.Get<YafBoardSettings>().UseStyledNicks)
                {
                    this.Get<IStyleTransform>().DecodeStyleByTable(ref activeTopics, true, "LastUserStyle");
                }

                if (this.PageContext.IsGuest)
                {
                    this.Get<IDataCache>().Set(
                      CacheKey, activeTopics, TimeSpan.FromMinutes(this.Get<YafBoardSettings>().ActiveDiscussionsCacheTimeout));
                }
            }

            this.CollapsibleImage.ToolTip = this.GetText("COMMON", "SHOWHIDE");

            bool groupAccess = this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().PostLatestFeedAccess);
            this.AtomFeed.Visible = this.Get<YafBoardSettings>().ShowAtomLink && groupAccess;
            this.RssFeed.Visible = this.Get<YafBoardSettings>().ShowRSSLink && groupAccess;

            this.lastPostToolTip = this.GetText("DEFAULT", "GO_LAST_POST");
            this.firstUnreadPostToolTip = this.GetText("DEFAULT", "GO_LASTUNREAD_POST");
            this.LatestPosts.DataSource = activeTopics;
            this.LatestPosts.DataBind();
        }

        #endregion
    }
}