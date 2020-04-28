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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;
    using YAF.Web.Extensions;

    using Forum = YAF.Types.Models.Forum;

    #endregion

    /// <summary>
    /// The topics list page
    /// </summary>
    public partial class Topics : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   The show topic list selected.
        /// </summary>
        private int showTopicListSelected;

        /// <summary>
        /// The forum.
        /// </summary>
        private Forum forum;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Topics" /> class. 
        ///   Overloads the topics page.
        /// </summary>
        public Topics()
            : base("TOPICS")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The style transform function wrap.
        /// </summary>
        /// <param name="dt">
        /// The DateTable
        /// </param>
        /// <returns>
        /// The style transform wrap.
        /// </returns>
        public DataTable StyleTransformDataTable([NotNull] DataTable dt)
        {
            if (!this.Get<BoardSettings>().UseStyledNicks)
            {
                return dt;
            }

            var styleTransform = this.Get<IStyleTransform>();
            styleTransform.DecodeStyleByTable(dt, false, new[] { "StarterStyle", "LastUserStyle" });

            return dt;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            this.PageContext.PageElements.RegisterJsBlockStartup(
                "TopicStarterPopoverJs",
                JavaScriptBlocks.TopicLinkPopoverJs(
                    $"{this.GetText("TOPIC_STARTER")}&nbsp;...",
                    ".topic-starter-popover",
                    "hover"));

            this.PageContext.PageElements.RegisterJsBlockStartup(
                "TopicLinkPopoverJs",
                JavaScriptBlocks.TopicLinkPopoverJs(
                    $"{this.GetText("LASTPOST")}&nbsp;{this.GetText("SEARCH", "BY")} ...",
                    ".topic-link-popover",
                    "focus hover"));

            var iconLegend = new IconLegend().RenderToString();

            this.PageContext.PageElements.RegisterJsBlockStartup(
                "TopicIconLegendPopoverJs",
                JavaScriptBlocks.ForumIconLegendPopoverJs(
                    iconLegend.ToJsString(),
                    "topic-icon-legend-popvover"));

            base.OnPreRender(e);
        }

        /// <summary>
        /// Gets the sub forum title.
        /// </summary>
        /// <returns>The get sub forum title.</returns>
            protected string GetSubForumTitle()
        {
            return this.GetTextFormatted("SUBFORUMS", this.HtmlEncode(this.PageContext.PageForumName));
        }

        /// <summary>
        /// The Forum Search
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ForumSearch_Click(object sender, EventArgs e)
        {
            if (this.forumSearch.Text.IsNotSet())
            {
                return;
            }

            BuildLink.Redirect(
                ForumPages.Search,
                "search={0}&forum={1}",
                this.forumSearch.Text,
                this.PageContext.PageForumID);
        }

        /// <summary>
        /// The initialization script for the topics page.
        /// </summary>
        /// <param name="e">
        /// The EventArgs object for the topics page.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.Unload += this.Topics_Unload;

            this.ShowList.SelectedIndexChanged += this.ShowList_SelectedIndexChanged;
            this.MarkRead.Click += this.MarkRead_Click;
            this.Pager.PageChange += this.Pager_PageChange;
            this.WatchForum.Click += this.WatchForum_Click;

            base.OnInit(e);
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Get<ISession>().UnreadTopics = 0;

            this.RssFeed.AdditionalParameters = $"f={this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f")}";

            this.ForumJumpHolder.Visible = this.Get<BoardSettings>().ShowForumJump
                                           && this.PageContext.Settings.LockedForum == 0;

            if (this.ForumSearchHolder.Visible)
            {
                this.forumSearch.Attributes["onkeydown"] =
                    $"if(event.which || event.keyCode){{if ((event.which == 13) || (event.keyCode == 13)) {{document.getElementById('{this.forumSearchOK.ClientID}').click();return false;}}}} else {{return true}}; ";
            }

            if (!this.IsPostBack)
            {
                this.ShowList.DataSource = StaticDataHelper.TopicTimes();
                this.ShowList.DataTextField = "Name";
                this.ShowList.DataValueField = "Value";
                this.showTopicListSelected = this.Get<ISession>().ShowList == -1
                                                  ? this.Get<BoardSettings>().ShowTopicsDefault
                                                  : this.Get<ISession>().ShowList;

                this.moderate1.NavigateUrl =
                    this.moderate2.NavigateUrl =
                    BuildLink.GetLinkNotEscaped(ForumPages.Moderating, "f={0}", this.PageContext.PageForumID);

                this.NewTopic1.NavigateUrl =
                    this.NewTopic2.NavigateUrl =
                    BuildLink.GetLinkNotEscaped(ForumPages.PostTopic, "f={0}", this.PageContext.PageForumID);

                this.HandleWatchForum();
            }

            if (!this.Get<HttpRequestBase>().QueryString.Exists("f"))
            {
                BuildLink.AccessDenied();
            }

            if (this.PageContext.IsGuest && !this.PageContext.ForumReadAccess)
            {
                // attempt to get permission by redirecting to login...
                this.Get<IPermissions>().HandleRequest(ViewPermissions.RegisteredUsers);
            }
            else if (!this.PageContext.ForumReadAccess)
            {
                BuildLink.AccessDenied();
            }

            this.forum = this.GetRepository<Forum>().GetById(this.PageContext.PageForumID);

            if (this.forum.RemoteURL.IsSet())
            {
                this.Get<HttpResponseBase>().Clear();
                this.Get<HttpResponseBase>().Redirect(this.forum.RemoteURL);
            }

            this.PageTitle.Text = this.forum.Description.IsSet()
                                      ? $"{this.HtmlEncode(this.forum.Name)} - <em>{this.HtmlEncode(this.forum.Description)}</em>"
                                      : this.HtmlEncode(this.forum.Name);

            this.BindData(); // Always because of yaf:TopicLine

            if (!this.PageContext.ForumPostAccess
                || this.forum.ForumFlags.IsLocked && !this.PageContext.ForumModeratorAccess)
            {
                this.NewTopic1.Visible = false;
                this.NewTopic2.Visible = false;
            }

            if (this.PageContext.IsGuest)
            {
                this.WatchForum.Visible = false;
                this.MarkRead.Visible = false;
            }

            if (this.PageContext.ForumModeratorAccess)
            {
                return;
            }

            this.moderate1.Visible = false;
            this.moderate2.Visible = false;
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // PageLinks.Clear();
            if (this.PageContext.Settings.LockedForum == 0)
            {
                this.PageLinks.AddRoot();
                this.PageLinks.AddLink(
                    this.PageContext.PageCategoryName,
                    BuildLink.GetLink(ForumPages.Board, "c={0}", this.PageContext.PageCategoryID));
            }

            this.PageLinks.AddForum(this.PageContext.PageForumID, true);
        }

        /// <summary>
        /// The create topic line.
        /// </summary>
        /// <param name="containerDataItem">
        /// The container data item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string CreateTopicLine(DataRowView containerDataItem)
        {
            var topicLine = new TopicContainer { DataRow = containerDataItem };

            return topicLine.RenderToString();
        }

        /// <summary>
        /// The watch forum_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void WatchForum_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageContext.ForumReadAccess)
            {
                return;
            }

            if (this.PageContext.IsGuest)
            {
                this.PageContext.AddLoadMessage(this.GetText("WARN_LOGIN_FORUMWATCH"), MessageTypes.warning);
                return;
            }

            if (this.WatchForum.Icon == "eye")
            {
                this.GetRepository<WatchForum>().Add(this.PageContext.PageUserID, this.PageContext.PageForumID);

                this.PageContext.AddLoadMessage(this.GetText("INFO_WATCH_FORUM"), MessageTypes.success);
            }
            else
            {
                this.GetRepository<WatchForum>().Delete(
                    w => w.ForumID == this.PageContext.PageForumID && w.UserID == this.PageContext.PageUserID);

                this.PageContext.AddLoadMessage(this.GetText("INFO_UNWATCH_FORUM"), MessageTypes.success);
            }

            this.HandleWatchForum();

            this.BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            var ds = this.Get<DataBroker>().BoardLayout(
                this.PageContext.PageBoardID,
                this.PageContext.PageUserID,
                this.PageContext.PageCategoryID,
                this.PageContext.PageForumID);
            if (ds.Tables["Forum"].HasRows())
            {
                this.ForumList.DataSource = ds.Tables["Forum"].Rows;
                this.SubForums.Visible = true;
            }

            this.Pager.PageSize = this.Get<BoardSettings>().TopicsPerPage;

            // when userId is null it returns the count of all deleted messages
            /*int? userId = null;

            // get the userID to use for the deleted posts count...
            if (!this.Get<BoardSettings>().ShowDeletedMessagesToAll)
            {
                // only show deleted messages that belong to this user if they are not admin/mod
                if (!this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess)
                {
                    userId = this.PageContext.PageUserID;
                }
            }*/
            int? userId = this.PageContext.PageUserID;

            var dt = this.GetRepository<Topic>().AnnouncementsAsDataTable(
                this.PageContext.PageForumID,
                userId,
                null,
                System.DateTime.UtcNow,
                0,
                10,
                this.Get<BoardSettings>().UseStyledNicks,
                true,
                this.Get<BoardSettings>().UseReadTrackingByDatabase);
            if (dt != null)
            {
                dt = this.StyleTransformDataTable(dt);
            }

            var baseSize = this.Get<BoardSettings>().TopicsPerPage;

            this.Announcements.DataSource = dt;

            var pagerCurrentPageIndex = this.Pager.CurrentPageIndex;

            DataTable topicList;

            if (this.showTopicListSelected == 0)
            {
                topicList = this.GetRepository<Topic>().ListAsDataTable(
                    this.PageContext.PageForumID,
                    userId,
                    DateTimeHelper.SqlDbMinTime(),
                    System.DateTime.UtcNow,
                    pagerCurrentPageIndex,
                    baseSize,
                    this.Get<BoardSettings>().UseStyledNicks,
                    true,
                    this.Get<BoardSettings>().UseReadTrackingByDatabase);
                if (topicList != null)
                {
                    topicList = this.StyleTransformDataTable(topicList);
                }
            }
            else
            {
                int[] days = { 1, 2, 7, 14, 31, 2 * 31, 6 * 31, 356 };

                var date = System.DateTime.UtcNow.AddDays(-days[this.showTopicListSelected]);

                topicList = this.GetRepository<Topic>().ListAsDataTable(
                    this.PageContext.PageForumID,
                    userId,
                    date,
                    System.DateTime.UtcNow,
                    pagerCurrentPageIndex,
                    baseSize,
                    this.Get<BoardSettings>().UseStyledNicks,
                    true,
                    this.Get<BoardSettings>().UseReadTrackingByDatabase);

                if (topicList != null)
                {
                    topicList = this.StyleTransformDataTable(topicList);
                }
            }

            this.TopicList.DataSource = topicList;

            this.DataBind();

            // setup the show topic list selection after data binding
            this.ShowList.SelectedIndex = this.showTopicListSelected;
            this.Get<ISession>().ShowList = this.showTopicListSelected;

            if (topicList != null && topicList.HasRows())
            {
                this.Pager.Count = topicList.AsEnumerable().First().Field<int>("TotalRows");
            }

            if (this.Announcements.Items.Count == 0 && this.TopicList.Items.Count == 0)
            {
                this.NoPostsPlaceHolder.Visible = true;
            }
            else
            {
                this.NoPostsPlaceHolder.Visible = false;
            }
        }

        /// <summary>
        /// The handle watch forum.
        /// </summary>
        private void HandleWatchForum()
        {
            if (this.PageContext.IsGuest || !this.PageContext.ForumReadAccess)
            {
                return;
            }

            // check if this forum is being watched by this user
            var watchForumId = this.GetRepository<WatchForum>().Check(this.PageContext.PageUserID, this.PageContext.PageForumID);

            if (watchForumId.HasValue)
            {
                // subscribed to this forum
                this.WatchForum.TextLocalizedTag = "UNWATCHFORUM";
                this.WatchForum.Icon = "eye-slash";
            }
            else
            {
                // not subscribed
                this.WatchForum.TextLocalizedTag = "WATCHFORUM";
                this.WatchForum.Icon = "eye";
            }
        }

        /// <summary>
        /// The mark read_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MarkRead_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Get<IReadTrackCurrentUser>().SetForumRead(this.PageContext.PageForumID);

            this.BindData();
        }

        /// <summary>
        /// The pager_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// The show list_ selected index changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ShowList_SelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.showTopicListSelected = this.ShowList.SelectedIndex;
            this.BindData();
        }

        /// <summary>
        /// The Topics unload.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Topics_Unload([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.Get<ISession>().UnreadTopics == 0)
            {
                this.Get<IReadTrackCurrentUser>().SetForumRead(this.PageContext.PageForumID);
            }
        }

        #endregion
    }
}