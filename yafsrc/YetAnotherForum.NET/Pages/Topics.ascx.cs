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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

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
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;
    using YAF.Types.Objects.Model;
    using YAF.Web.Controls;
    using YAF.Web.Extensions;

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

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Topics" /> class.
        ///   Overloads the topics page.
        /// </summary>
        public Topics()
            : base("TOPICS", ForumPages.Topics)
        {
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
            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                "TopicStarterPopoverJs",
                JavaScriptBlocks.TopicLinkPopoverJs(
                    $"{this.GetText("TOPIC_STARTER")}&nbsp;...",
                    ".topic-starter-popover",
                    "hover"));

            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                "TopicLinkPopoverJs",
                JavaScriptBlocks.TopicLinkPopoverJs(
                    $"{this.GetText("LASTPOST")}&nbsp;{this.GetText("SEARCH", "BY")} ...",
                    ".topic-link-popover",
                    "focus hover"));

            var iconLegend = new IconLegend().RenderToString();

            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                "TopicIconLegendPopoverJs",
                JavaScriptBlocks.ForumIconLegendPopoverJs(
                    iconLegend.ToJsString(),
                    "topic-icon-legend-popvover"));

            this.PageBoardContext.PageElements.RegisterJsBlock("dropDownToggleJs", JavaScriptBlocks.DropDownToggleJs());

            base.OnPreRender(e);
        }

        /// <summary>
        /// Gets the sub forum title.
        /// </summary>
        /// <returns>The get sub forum title.</returns>
        protected string GetSubForumTitle()
        {
            return this.GetTextFormatted("SUBFORUMS", this.HtmlEncode(this.PageBoardContext.PageForum.Name));
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

            this.Get<LinkBuilder>().Redirect(
                ForumPages.Search,
                new { search = this.forumSearch.Text, forum = this.PageBoardContext.PageForumID} );
        }

        /// <summary>
        /// The initialization script for the topics page.
        /// </summary>
        /// <param name="e">
        /// The EventArgs object for the topics page.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            // in case topic is deleted or not existent
            if (this.PageBoardContext.PageForum == null)
            {
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            }

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

            this.ForumJumpHolder.Visible = this.PageBoardContext.BoardSettings.ShowForumJump
                                           && this.PageBoardContext.Settings.LockedForum == 0;

            this.ForumSearchHolder.Visible =
                this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.SearchPermissions);

            if (this.ForumSearchHolder.Visible)
            {
                this.forumSearch.Attributes.Add(
                    "onkeydown",
                    JavaScriptBlocks.ClickOnEnterJs(this.forumSearchOK.ClientID));
            }

            if (!this.IsPostBack)
            {
                this.ShowList.DataSource = StaticDataHelper.TopicTimes();
                this.ShowList.DataTextField = "Name";
                this.ShowList.DataValueField = "Value";
                this.showTopicListSelected = this.Get<ISession>().ShowList == -1
                                                  ? this.PageBoardContext.BoardSettings.ShowTopicsDefault
                                                  : this.Get<ISession>().ShowList;

                this.moderate1.NavigateUrl =
                    this.moderate2.NavigateUrl =
                    this.Get<LinkBuilder>().GetLink(ForumPages.Moderate_Forums, new { f = this.PageBoardContext.PageForumID });

                this.NewTopic1.NavigateUrl =
                    this.NewTopic2.NavigateUrl =
                    this.Get<LinkBuilder>().GetLink(ForumPages.PostTopic, new { f = this.PageBoardContext.PageForumID });

                this.HandleWatchForum();
            }

            if (!this.Get<HttpRequestBase>().QueryString.Exists("f"))
            {
                this.Get<LinkBuilder>().AccessDenied();
            }

            if (this.PageBoardContext.IsGuest && !this.PageBoardContext.ForumReadAccess)
            {
                // attempt to get permission by redirecting to login...
                this.Get<IPermissions>().HandleRequest(ViewPermissions.RegisteredUsers);
            }
            else if (!this.PageBoardContext.ForumReadAccess)
            {
                this.Get<LinkBuilder>().AccessDenied();
            }

            if (this.PageBoardContext.PageForum.RemoteURL.IsSet())
            {
                this.Get<HttpResponseBase>().Clear();
                this.Get<HttpResponseBase>().Redirect(this.PageBoardContext.PageForum.RemoteURL);
            }

            this.PageTitle.Text = this.PageBoardContext.PageForum.Description.IsSet()
                ? $"{this.HtmlEncode(this.PageBoardContext.PageForum.Name)} - <em>{this.HtmlEncode(this.PageBoardContext.PageForum.Description)}</em>"
                : this.HtmlEncode(this.PageBoardContext.PageForum.Name);

            this.PageSize.DataSource = StaticDataHelper.PageEntries();
            this.PageSize.DataTextField = "Name";
            this.PageSize.DataValueField = "Value";
            this.PageSize.DataBind();

            try
            {
                this.PageSize.SelectedValue = this.PageBoardContext.PageUser.PageSize.ToString();
            }
            catch (Exception)
            {
                this.PageSize.SelectedValue = "5";
            }

            this.BindData(); // Always because of yaf:TopicLine

            if (!this.PageBoardContext.ForumPostAccess
                || this.PageBoardContext.PageForum.ForumFlags.IsLocked && !this.PageBoardContext.ForumModeratorAccess)
            {
                this.NewTopic1.Visible = false;
                this.NewTopic2.Visible = false;
            }

            if (this.PageBoardContext.IsGuest)
            {
                this.WatchForum.Visible = false;
                this.MarkRead.Visible = false;
            }

            if (this.PageBoardContext.ForumModeratorAccess)
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
            if (this.PageBoardContext.Settings.LockedForum == 0)
            {
                this.PageLinks.AddRoot();
                this.PageLinks.AddCategory(this.PageBoardContext.PageCategory.Name, this.PageBoardContext.PageCategoryID);
            }

            this.PageLinks.AddForum(this.PageBoardContext.PageForumID, true);
        }

        /// <summary>
        /// The create topic line.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string CreateTopicLine(object item)
        {
            var topicLine = new TopicContainer { Item = item as PagedTopic };

            return topicLine.RenderToString();
        }

        /// <summary>
        /// The watch forum_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void WatchForum_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageBoardContext.ForumReadAccess)
            {
                return;
            }

            if (this.PageBoardContext.IsGuest)
            {
                this.PageBoardContext.Notify(this.GetText("WARN_LOGIN_FORUMWATCH"), MessageTypes.warning);
                return;
            }

            if (this.WatchForum.Icon == "eye")
            {
                this.GetRepository<WatchForum>().Add(this.PageBoardContext.PageUserID, this.PageBoardContext.PageForumID);

                this.PageBoardContext.Notify(this.GetText("INFO_WATCH_FORUM"), MessageTypes.success);
            }
            else
            {
                this.GetRepository<WatchForum>().Delete(
                    w => w.ForumID == this.PageBoardContext.PageForumID && w.UserID == this.PageBoardContext.PageUserID);

                this.PageBoardContext.Notify(this.GetText("INFO_UNWATCH_FORUM"), MessageTypes.success);
            }

            this.HandleWatchForum();

            this.BindData();
        }

        /// <summary>
        /// The page size on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PageSizeSelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            var forums = this.Get<DataBroker>().BoardLayout(
                this.PageBoardContext.PageBoardID,
                this.PageBoardContext.PageUserID,
                this.PageBoardContext.PageCategoryID,
                this.PageBoardContext.PageForumID);

            // Render Sub forum(s)
            if (forums.Item2.Any())
            {
                this.ForumList.DataSource = forums;
                this.SubForums.Visible = true;
            }

            var baseSize = this.PageSize.SelectedValue.ToType<int>();

            this.Pager.PageSize = baseSize;

            var list = this.GetRepository<Topic>().ListAnnouncementsPaged(
                this.PageBoardContext.PageForumID,
                this.PageBoardContext.PageUserID,
                0,
                10,
                this.PageBoardContext.BoardSettings.UseReadTrackingByDatabase);

            this.Announcements.DataSource = list;

            var pagerCurrentPageIndex = this.Pager.CurrentPageIndex;

            int[] days = { 1, 2, 7, 14, 31, 2 * 31, 6 * 31, 356 };

            var topicList = this.GetRepository<Topic>().ListPaged(
                this.PageBoardContext.PageForumID,
                this.PageBoardContext.PageUserID,
                this.showTopicListSelected == 0 ? DateTimeHelper.SqlDbMinTime() : DateTime.UtcNow.AddDays(-days[this.showTopicListSelected]),
                pagerCurrentPageIndex,
                baseSize,
                this.PageBoardContext.BoardSettings.UseReadTrackingByDatabase);

            this.TopicList.DataSource = topicList;

            this.DataBind();

            // setup the show topic list selection after data binding
            this.ShowList.SelectedIndex = this.showTopicListSelected;
            this.Get<ISession>().ShowList = this.showTopicListSelected;

            if (topicList != null && topicList.Any())
            {
                this.Pager.Count = topicList.FirstOrDefault().TotalRows;
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
            if (this.PageBoardContext.IsGuest || !this.PageBoardContext.ForumReadAccess)
            {
                return;
            }

            // check if this forum is being watched by this user
            var watchForumId = this.GetRepository<WatchForum>().Check(this.PageBoardContext.PageUserID, this.PageBoardContext.PageForumID);

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
            this.Get<IReadTrackCurrentUser>().SetForumRead(this.PageBoardContext.PageForumID);

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
                this.Get<IReadTrackCurrentUser>().SetForumRead(this.PageBoardContext.PageForumID);
            }
        }

        #endregion
    }
}