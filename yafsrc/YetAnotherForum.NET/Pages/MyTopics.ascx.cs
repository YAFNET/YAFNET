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

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The my topics page.
    /// </summary>
    public partial class MyTopics : ForumPageRegistered
    {
        /// <summary>
        /// Indicates if the Active Tab was loaded
        /// </summary>
        private bool activeLoaded;

        /// <summary>
        /// Indicates if the Unanswered Tab was loaded
        /// </summary>
        private bool unansweredLoaded;

        /// <summary>
        /// Indicates if the Unread Tab was loaded
        /// </summary>
        private bool unreadLoaded;

        /// <summary>
        /// Indicates if the My Topics Tab was loaded
        /// </summary>
        private bool myTopicsLoaded;

        /// <summary>
        /// Indicates if the Favorite Tab was loaded
        /// </summary>
        private bool favoriteLoaded;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MyTopics"/> class.
        /// </summary>
        public MyTopics()
            : base("MYTOPICS")
        {
        }

        #endregion

        /// <summary>
        /// Gets or sets the current tab.
        /// </summary>
        /// <value>
        /// The current tab.
        /// </value>
        private TopicListMode CurrentTab
        {
            get
            {
                if (this.ViewState["CurrentTab"] != null)
                {
                    return (TopicListMode)this.ViewState["CurrentTab"];
                }

                return TopicListMode.Active;
            }

            set => this.ViewState["CurrentTab"] = value;
        }

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

            this.PageContext.PageElements.RegisterJsBlock(
                "TopicsTabsJs",
                JavaScriptBlocks.BootstrapTabsLoadJs(
                    this.TopicsTabs.ClientID,
                    this.hidLastTab.ClientID,
                    this.Page.ClientScript.GetPostBackEventReference(this.ChangeTab, string.Empty)));

            var iconLegend = new IconLegend().RenderToString();

            this.PageContext.PageElements.RegisterJsBlockStartup(
                "TopicIconLegendPopoverJs",
                JavaScriptBlocks.ForumIconLegendPopoverJs(
                    iconLegend.ToJsString(),
                    "topic-icon-legend-popvover"));

            base.OnPreRender(e);
        }

        /// <summary>
        /// The Page_ Load Event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                this.RefreshTab();

                return;
            }

            this.UnreadTopicsTabTitle.Visible = this.Get<BoardSettings>().UseReadTrackingByDatabase;
            this.UnreadTopicsTabContent.Visible = this.Get<BoardSettings>().UseReadTrackingByDatabase;
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();

            this.PageLinks.AddLink(this.GetText("MEMBERTITLE"), string.Empty);
        }

        #endregion

        /// <summary>
        /// Load the Selected Tab Content
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ChangeTabClick(object sender, EventArgs e)
        {
            this.CurrentTab = this.hidLastTab.Value switch
                {
                    "UnansweredTopicsTab" => TopicListMode.Unanswered,
                    "UnreadTopicsTab" => TopicListMode.Unread,
                    "MyTopicsTab" => TopicListMode.User,
                    "FavoriteTopicsTab" => TopicListMode.Favorite,
                    _ => TopicListMode.Active
                };

            this.RefreshTab();
        }

        /// <summary>
        /// Refreshes the tab.
        /// </summary>
        private void RefreshTab()
        {
            switch (this.CurrentTab)
            {
                case TopicListMode.Unanswered:

                    if (!this.unansweredLoaded)
                    {
                        this.UnansweredTopics.BindData();

                        this.ActiveTopics.DataBind();
                        if (this.UnreadTopicsTabTitle.Visible)
                        {
                            this.UnreadTopics.DataBind();
                        }

                        if (this.UserTopicsTabTitle.Visible)
                        {
                            this.MyTopicsTopics.DataBind();
                            this.FavoriteTopics.DataBind();
                        }

                        this.unansweredLoaded = true;
                    }

                    break;
                case TopicListMode.Unread:

                    if (!this.unreadLoaded)
                    {
                        this.UnreadTopics.BindData();

                        this.ActiveTopics.DataBind();
                        this.UnansweredTopics.DataBind();

                        if (this.UserTopicsTabTitle.Visible)
                        {
                            this.MyTopicsTopics.DataBind();
                            this.FavoriteTopics.DataBind();
                        }

                        this.unreadLoaded = true;
                    }

                    break;
                case TopicListMode.User:

                    if (!this.myTopicsLoaded)
                    {
                        this.MyTopicsTopics.BindData();

                        this.ActiveTopics.DataBind();
                        this.UnansweredTopics.DataBind();
                        if (this.UnreadTopicsTabTitle.Visible)
                        {
                            this.UnreadTopics.DataBind();
                        }

                        if (this.UserTopicsTabTitle.Visible)
                        {
                            this.FavoriteTopics.DataBind();
                        }

                        this.myTopicsLoaded = true;
                    }
                    else
                    {
                        this.MyTopicsTopics.DataBind();

                        this.ActiveTopics.DataBind();
                        this.UnansweredTopics.DataBind();
                        if (this.UnreadTopicsTabTitle.Visible)
                        {
                            this.UnreadTopics.DataBind();
                        }

                        if (this.UserTopicsTabTitle.Visible)
                        {
                            this.FavoriteTopics.DataBind();
                        }
                    }

                    break;
                case TopicListMode.Favorite:

                    if (!this.favoriteLoaded)
                    {
                        this.FavoriteTopics.BindData();

                        this.ActiveTopics.DataBind();
                        this.UnansweredTopics.DataBind();
                        if (this.UnreadTopicsTabTitle.Visible)
                        {
                            this.UnreadTopics.DataBind();
                        }

                        if (this.UserTopicsTabTitle.Visible)
                        {
                            this.MyTopicsTopics.DataBind();
                        }

                        this.favoriteLoaded = true;
                    }

                    break;
                case TopicListMode.Active:

                    if (!this.activeLoaded)
                    {
                        this.ActiveTopics.DataBind();
                        this.UnansweredTopics.DataBind();
                        if (this.UnreadTopicsTabTitle.Visible)
                        {
                            this.UnreadTopics.DataBind();
                        }

                        if (this.UserTopicsTabTitle.Visible)
                        {
                            this.MyTopicsTopics.DataBind();
                            this.FavoriteTopics.DataBind();
                        }

                        this.activeLoaded = true;
                    }

                    break;
            }
        }
    }
}