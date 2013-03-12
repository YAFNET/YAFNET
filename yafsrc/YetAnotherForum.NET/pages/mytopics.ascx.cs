/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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
namespace YAF.Pages
{
    #region Using

    using System;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The my topics page.
    /// </summary>
    public partial class mytopics : ForumPage
    {
        /// <summary>
        /// Indicates if the Active Tab was loaded
        /// </summary>
        private bool activeloaded;

        /// <summary>
        /// Indicates if the Unanswered Tab was loaded
        /// </summary>
        private bool unansweredloaded;

        /// <summary>
        /// Indicates if the Unread Tab was loaded
        /// </summary>
        private bool unreadloaded;

        /// <summary>
        /// Indicates if the My Topics Tab was loaded
        /// </summary>
        private bool mytopicsloaded;

        /// <summary>
        /// Indicates if the Favorite Tab was loaded
        /// </summary>
        private bool favoriteloaded;

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

            set
            {
                this.ViewState["CurrentTab"] = value;
            }
        }

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the mytopics class.
        /// </summary>
        public mytopics()
            : base("MYTOPICS")
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
            // setup jQuery and Jquery Ui Tabs.
            YafContext.Current.PageElements.RegisterJQuery();
            YafContext.Current.PageElements.RegisterJQueryUI();

            YafContext.Current.PageElements.RegisterJsBlock(
                "TopicsTabsJs",
                JavaScriptBlocks.JqueryUITabsLoadJs(
                    this.TopicsTabs.ClientID,
                    this.hidLastTab.ClientID,
                    this.hidLastTabId.ClientID,
                    this.Page.ClientScript.GetPostBackEventReference(ChangeTab, string.Empty),
                    false,
                    true));

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

            this.UserTopicsTabTitle.Visible = !this.PageContext.IsGuest;
            this.UserTopicsTabContent.Visible = !this.PageContext.IsGuest;

            this.UnreadTopicsTabTitle.Visible = !this.PageContext.IsGuest &&
                                                this.Get<YafBoardSettings>().UseReadTrackingByDatabase;
            this.UnreadTopicsTabContent.Visible = !this.PageContext.IsGuest &&
                                                  this.Get<YafBoardSettings>().UseReadTrackingByDatabase;

            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));

            this.PageLinks.AddLink(
                this.PageContext.IsGuest ? this.GetText("GUESTTITLE") : this.GetText("MEMBERTITLE"), string.Empty);

            this.ForumJumpHolder.Visible = this.Get<YafBoardSettings>().ShowForumJump &&
                                           this.PageContext.Settings.LockedForum == 0;
        }

        #endregion

        /// <summary>
        /// Load the Selected Tab Content
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ChangeTabClick(object sender, EventArgs e)
        {
            switch (hidLastTabId.Value)
            {
                case "UnansweredTopicsTab":
                    this.CurrentTab = TopicListMode.Unanswered;
                    break;
                case "UnreadTopicsTab":
                    this.CurrentTab = TopicListMode.Unread;
                    break;
                case "MyTopicsTab":
                    this.CurrentTab = TopicListMode.User;
                    break;
                case "FavoriteTopicsTab":
                    this.CurrentTab = TopicListMode.Favorite;
                    break;
                default:
                    this.CurrentTab = TopicListMode.Active;
                    break;
            }

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

                    if (!this.unansweredloaded)
                    {
                        this.UnansweredTopics.BindData();

                        this.ActiveTopics.DataBind();
                        if (this.UnreadTopicsTabTitle.Visible)
                        {
                            this.UnreadTopics.DataBind();
                        }

                        if (this.UserTopicsTabTitle.Visible)
                        {
                            this.MyTopics.DataBind();
                            this.FavoriteTopics.DataBind();
                        }

                        this.unansweredloaded = true;
                    }

                    break;
                case TopicListMode.Unread:

                    if (!this.unreadloaded)
                    {
                        this.UnreadTopics.BindData();

                        this.ActiveTopics.DataBind();
                        this.UnansweredTopics.DataBind();

                        if (this.UserTopicsTabTitle.Visible)
                        {
                            this.MyTopics.DataBind();
                            this.FavoriteTopics.DataBind();
                        }

                        this.unreadloaded = true;
                    }

                    break;
                case TopicListMode.User:

                    if (!this.mytopicsloaded)
                    {
                        this.MyTopics.BindData();

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

                        this.mytopicsloaded = true;
                    }
                    else
                    {
                        this.MyTopics.DataBind();

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

                    if (!this.favoriteloaded)
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
                            this.MyTopics.DataBind();
                        }

                        this.favoriteloaded = true;
                    }

                    break;
                case TopicListMode.Active:

                    if (!this.activeloaded)
                    {
                        this.ActiveTopics.DataBind();
                        this.UnansweredTopics.DataBind();
                        if (this.UnreadTopicsTabTitle.Visible)
                        {
                            this.UnreadTopics.DataBind();
                        }

                        if (this.UserTopicsTabTitle.Visible)
                        {
                            this.MyTopics.DataBind();
                            this.FavoriteTopics.DataBind();
                        }

                        this.activeloaded = true;
                    }

                    break;
            }
        }
    }
}