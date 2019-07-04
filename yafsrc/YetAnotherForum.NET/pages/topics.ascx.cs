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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Web;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The topics list page
    /// </summary>
    public partial class topics : ForumPage
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
        ///   Initializes a new instance of the <see cref = "topics" /> class. 
        ///   Overloads the topics page.
        /// </summary>
        public topics()
            : base("TOPICS")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the last post image TT.
        /// </summary>
        /// <value>
        /// The last post image TT.
        /// </value>
        public string LastPostImageTT { get; set; }

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
            if (!this.Get<YafBoardSettings>().UseStyledNicks)
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
        /// Gets the sub forum title.
        /// </summary>
        /// <returns>The get sub forum title.</returns>
        protected string GetSubForumTitle()
        {
            return this.GetTextFormatted("SUBFORUMS", this.HtmlEncode(this.PageContext.PageForumName));
        }

        /// <summary>
        /// The new topic_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void NewTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.forum.ForumFlags.IsLocked)
            {
                this.PageContext.AddLoadMessage(this.GetText("WARN_FORUM_LOCKED"));
                return;
            }

            if (!this.PageContext.ForumPostAccess)
            {
                YafBuildLink.AccessDenied(/*"You don't have access to post new topics in this forum."*/);
            }
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

            YafBuildLink.Redirect(
                ForumPages.search,
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
            this.NewTopic1.Click += this.NewTopic_Click;
            this.NewTopic2.Click += this.NewTopic_Click;
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
            this.Get<IYafSession>().UnreadTopics = 0;

            this.RssFeed.AdditionalParameters = $"f={this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f")}";

            this.ForumJumpHolder.Visible = this.Get<YafBoardSettings>().ShowForumJump
                                           && this.PageContext.Settings.LockedForum == 0;

            this.LastPostImageTT = this.GetText("DEFAULT", "GO_LAST_POST");

            if (this.ForumSearchHolder.Visible)
            {
                this.forumSearch.Attributes["onkeydown"] =
                    $"if(event.which || event.keyCode){{if ((event.which == 13) || (event.keyCode == 13)) {{document.getElementById('{this.forumSearchOK.ClientID}').click();return false;}}}} else {{return true}}; ";
            }

            if (!this.IsPostBack)
            {
                // PageLinks.Clear();
                if (this.PageContext.Settings.LockedForum == 0)
                {
                    this.PageLinks.AddRoot();
                    this.PageLinks.AddLink(
                        this.PageContext.PageCategoryName,
                        YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
                }

                this.PageLinks.AddForum(this.PageContext.PageForumID, true);

                this.ShowList.DataSource = StaticDataHelper.TopicTimes();
                this.ShowList.DataTextField = "TopicText";
                this.ShowList.DataValueField = "TopicValue";
                this.showTopicListSelected = this.Get<IYafSession>().ShowList == -1
                                                  ? this.Get<YafBoardSettings>().ShowTopicsDefault
                                                  : this.Get<IYafSession>().ShowList;

                this.moderate1.NavigateUrl =
                    this.moderate2.NavigateUrl =
                    YafBuildLink.GetLinkNotEscaped(ForumPages.moderating, "f={0}", this.PageContext.PageForumID);

                this.NewTopic1.NavigateUrl =
                    this.NewTopic2.NavigateUrl =
                    YafBuildLink.GetLinkNotEscaped(ForumPages.postmessage, "f={0}", this.PageContext.PageForumID);

                this.HandleWatchForum();
            }

            if (this.Request.QueryString.GetFirstOrDefault("f") == null)
            {
                YafBuildLink.AccessDenied();
            }

            if (this.PageContext.IsGuest && !this.PageContext.ForumReadAccess)
            {
                // attempt to get permission by redirecting to login...
                this.Get<IPermissions>().HandleRequest(ViewPermissions.RegisteredUsers);
            }
            else if (!this.PageContext.ForumReadAccess)
            {
                YafBuildLink.AccessDenied();
            }

            var dt = this.GetRepository<Forum>().List(
                this.PageContext.PageBoardID,
                this.PageContext.PageForumID);

            this.forum = dt.FirstOrDefault();

            if (this.forum.RemoteURL.IsSet())
            {
                this.Response.Clear();
                this.Response.Redirect(this.forum.RemoteURL);
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

            if (this.PageContext.ForumModeratorAccess)
            {
                return;
            }

            this.moderate1.Visible = false;
            this.moderate2.Visible = false;
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
                this.PageContext.AddLoadMessage(this.GetText("WARN_LOGIN_FORUMWATCH"));
                return;
            }

            if (this.WatchForum.Icon == "eye")
            {
                this.GetRepository<WatchForum>().Add(this.PageContext.PageUserID, this.PageContext.PageForumID);

                this.PageContext.AddLoadMessage(this.GetText("INFO_WATCH_FORUM"), MessageTypes.success);
            }
            else
            {
                var watch = this.GetRepository<WatchForum>().Check(this.PageContext.PageUserID, this.PageContext.PageForumID);

                if (watch != null)
                {
                    this.GetRepository<WatchForum>().DeleteById(watch.Value);

                    this.PageContext.AddLoadMessage(this.GetText("INFO_UNWATCH_FORUM"), MessageTypes.success);
                }
            }

            this.HandleWatchForum();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            var ds = this.Get<YafDbBroker>().BoardLayout(
                this.PageContext.PageBoardID,
                this.PageContext.PageUserID,
                this.PageContext.PageCategoryID,
                this.PageContext.PageForumID);
            if (ds.Tables["Forum"].HasRows())
            {
                this.ForumList.DataSource = ds.Tables["Forum"].Rows;
                this.SubForums.Visible = true;
            }

            this.Pager.PageSize = this.Get<YafBoardSettings>().TopicsPerPage;

            // when userId is null it returns the count of all deleted messages
            /*int? userId = null;

            // get the userID to use for the deleted posts count...
            if (!this.Get<YafBoardSettings>().ShowDeletedMessagesToAll)
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
                DateTime.UtcNow,
                0,
                10,
                this.Get<YafBoardSettings>().UseStyledNicks,
                true,
                this.Get<YafBoardSettings>().UseReadTrackingByDatabase);
            if (dt != null)
            {
                dt = this.StyleTransformDataTable(dt);
            }

            var baseSize = this.Get<YafBoardSettings>().TopicsPerPage;

            this.Announcements.DataSource = dt;

            /*if (!m_bIgnoreQueryString && Request.QueryString["p"] != null)
                        {
                                // show specific page (p is 1 based)
                                int tPage = (int)Security.StringToLongOrRedirect(Request.QueryString["p"]);
            
                                if (tPage > 0)
                                {
                                        Pager.CurrentPageIndex = tPage - 1;
                                }
                        }*/
            var pagerCurrentPageIndex = this.Pager.CurrentPageIndex;

            DataTable topicList;

            if (this.showTopicListSelected == 0)
            {
                topicList = this.GetRepository<Topic>().ListAsDataTable(
                    this.PageContext.PageForumID,
                    userId,
                    DateTimeHelper.SqlDbMinTime(),
                    DateTime.UtcNow,
                    pagerCurrentPageIndex,
                    baseSize,
                    this.Get<YafBoardSettings>().UseStyledNicks,
                    true,
                    this.Get<YafBoardSettings>().UseReadTrackingByDatabase);
                if (topicList != null)
                {
                    topicList = this.StyleTransformDataTable(topicList);
                }
            }
            else
            {
                int[] days = { 1, 2, 7, 14, 31, 2 * 31, 6 * 31, 356 };

                var date = DateTime.UtcNow.AddDays(-days[this.showTopicListSelected]);

                topicList = this.GetRepository<Topic>().ListAsDataTable(
                    this.PageContext.PageForumID,
                    userId,
                    date,
                    DateTime.UtcNow,
                    pagerCurrentPageIndex,
                    baseSize,
                    this.Get<YafBoardSettings>().UseStyledNicks,
                    true,
                    this.Get<YafBoardSettings>().UseReadTrackingByDatabase);

                if (topicList != null)
                {
                    topicList = this.StyleTransformDataTable(topicList);
                }
            }

            this.TopicList.DataSource = topicList;

            this.DataBind();

            // setup the show topic list selection after data binding
            this.ShowList.SelectedIndex = this.showTopicListSelected;
            this.Get<IYafSession>().ShowList = this.showTopicListSelected;

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
            if (this.Get<IYafSession>().UnreadTopics == 0)
            {
                this.Get<IReadTrackCurrentUser>().SetForumRead(this.PageContext.PageForumID);
            }
        }

        #endregion
    }
}