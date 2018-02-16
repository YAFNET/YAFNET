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
 * to you under the Apache License, Version 2.0 (thehttp://localhost:50165/controls/AdminHeader.ascx.cs
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
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Dialogs;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Header.
    /// </summary>
    public partial class AdminHeader : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// The get return url.
        /// </summary>
        /// <returns>
        /// The url.
        /// </returns>
        protected string GetReturnUrl()
        {
            var returnUrl = string.Empty;

            if (this.PageContext.ForumPageType != ForumPages.login)
            {
                returnUrl = HttpContext.Current.Server.UrlEncode(General.GetSafeRawUrl());
            }
            else
            {
                // see if there is already one since we are on the login page
                if (HttpContext.Current.Request.QueryString.GetFirstOrDefault("ReturnUrl").IsSet())
                {
                    returnUrl =
                      HttpContext.Current.Server.UrlEncode(
                        General.GetSafeRawUrl(HttpContext.Current.Request.QueryString.GetFirstOrDefault("ReturnUrl")));
                }
            }

            return returnUrl;
        }

        /// <summary>
        /// Do Logout Dialog
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void LogOutClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            var notification = this.PageContext.CurrentForumPage.Notification.ToType<DialogBox>();

            notification.Show(
                this.GetText("TOOLBAR", "LOGOUT_QUESTION"),
                "Logout?",
                new DialogBox.DialogButton
                    {
                        Text = this.GetText("TOOLBAR", "LOGOUT"),
                        CssClass = "btn btn-primary",
                        ForumPageLink = new DialogBox.ForumLink { ForumPage = ForumPages.logout }
                    },
                new DialogBox.DialogButton
                    {
                        Text = this.GetText("COMMON", "CANCEL"),
                        CssClass = "btn btn-secondary"
                    });
        }

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RenderUserContainer();

            this.RenderMainHeaderMenu();

            this.RenderAdminModMenu();
        }

        /// <summary>
        /// Render Li and a Item
        /// </summary>
        /// <param name="holder">
        /// The holder.
        /// </param>
        /// <param name="linkText">
        /// The link text.
        /// </param>
        /// <param name="linkToolTip">
        /// The link tool tip.
        /// </param>
        /// <param name="linkUrl">
        /// The link URL.
        /// </param>
        /// <param name="noFollow">
        /// Add rel="nofollow" to the link
        /// </param>
        /// <param name="showUnread">
        /// The show unread.
        /// </param>
        /// <param name="unread">
        /// The unread.
        /// </param>
        /// <param name="unreadText">
        /// The unread text.
        /// </param>
        private static void RenderMenuItem(
            Control holder, string linkText, string linkToolTip, string linkUrl, bool noFollow, bool showUnread, string unread, string unreadText)
        {
            var element = new HtmlGenericControl("li");

            element.Attributes.Add("class", "dropdown-item");

            if (linkToolTip.IsNotSet())
            {
                linkToolTip = linkText;
            }

            var link = new HyperLink
            {
                Target = "_top",
                ToolTip = linkToolTip,
                NavigateUrl = linkUrl,
                Text = linkText
            };

            if (noFollow)
            {
                link.Attributes.Add("rel", "nofollow");
            }

            var unreadButton = new HtmlGenericControl("span");

            if (showUnread)
            {
                element.Controls.Add(unreadButton);
                unreadButton.Controls.Add(link);

                var unreadLabel = new HtmlGenericControl("span");

                unreadLabel.Attributes.Add("class", "badge badge-danger");

                unreadLabel.Attributes.Add("title", unreadText);
                
                unreadLabel.InnerText = unread;

                var unreadLabelText = new HtmlGenericControl("span");

                unreadLabelText.Attributes.Add("class", "sr-only");

                unreadLabelText.InnerText = unreadText;

                unreadButton.Controls.Add(unreadLabel);

                unreadButton.Controls.Add(unreadLabelText);
            }
            else
            {
                element.Controls.Add(link);
            }

            holder.Controls.Add(element);
        }

        /// <summary>
        /// Render the Admin/Moderator Menu Links
        /// </summary>
        private void RenderAdminModMenu()
        {
            // Admin
            if (this.PageContext.IsAdmin)
            {
                // Admin
                RenderMenuItem(
                    this.menuAdminItems,
                    "<i class=\"fa fa-tachometer-alt fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("TOOLBAR", "ADMIN")),
                    this.GetText("TOOLBAR", "ADMIN_TITLE"),
                    YafBuildLink.GetLink(ForumPages.admin_admin),
                    false,
                    false,
                    null,
                    null);
            }

            // Host
            if (this.PageContext.IsHostAdmin)
            {
                // Admin
                RenderMenuItem(
                    this.menuAdminItems,
                    "<i class=\"fa fa-cog fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("TOOLBAR", "HOST")),
                    this.GetText("TOOLBAR", "HOST_TITLE"),
                    YafBuildLink.GetLink(ForumPages.admin_hostsettings),
                    false,
                    false,
                    null,
                    null);
            }

            // Moderate
            if (this.PageContext.IsModeratorInAnyForum)
            {
                // Admin
                RenderMenuItem(
                    this.menuAdminItems,
                    "<i class=\"fa fa-university fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("TOOLBAR", "MODERATE")),
                    this.GetText("TOOLBAR", "MODERATE_TITLE"),
                    YafBuildLink.GetLink(ForumPages.moderate_index),
                    false,
                    this.PageContext.ModeratePosts > 0,
                    this.PageContext.ModeratePosts.ToString(),
                    this.GetText("TOOLBAR", "MODERATE_NEW").FormatWith(this.PageContext.ModeratePosts));
            }
        }

        /// <summary>
        /// Render the Main Header Menu Links
        /// </summary>
        private void RenderMainHeaderMenu()
        {
            // Forum
            RenderMenuItem(
                    this.menuListItems,
                    "<i class=\"fa fa-comments fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("DEFAULT", "FORUM")),
                    this.GetText("TOOLBAR", "FORUM_TITLE"),
                    YafBuildLink.GetLink(ForumPages.forum),
                    false,
                    false,
                    null,
                    null);

            // Active Topics
            if (this.PageContext.IsGuest)
            {
                RenderMenuItem(
                    this.menuListItems,
                    "<i class=\"fa fa-comment fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("TOOLBAR", "ACTIVETOPICS")),
                    this.GetText("TOOLBAR", "ACTIVETOPICS_TITLE"),
                    YafBuildLink.GetLink(ForumPages.mytopics),
                    false,
                    false,
                    null,
                    null);
            }

            // Search
            if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().SearchPermissions))
            {
                RenderMenuItem(
                    this.menuListItems,
                    "<i class=\"fa fa-search fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("TOOLBAR", "SEARCH")),
                    this.GetText("TOOLBAR", "SEARCH_TITLE"),
                    YafBuildLink.GetLink(ForumPages.search),
                    false,
                    false,
                    null,
                    null);
            }

            // Members
            if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().MembersListViewPermissions))
            {
                RenderMenuItem(
                    this.menuListItems,
                    "<i class=\"fa fa-users fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("TOOLBAR", "MEMBERS")),
                    this.GetText("TOOLBAR", "MEMBERS_TITLE"),
                    YafBuildLink.GetLink(ForumPages.members),
                    false,
                    false,
                    null,
                    null);
            }

            // Team
            if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ShowTeamTo))
            {
                RenderMenuItem(
                    this.menuListItems,
                    "<i class=\"fa fa-users fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("TOOLBAR", "TEAM")),
                    this.GetText("TOOLBAR", "TEAM_TITLE"),
                    YafBuildLink.GetLink(ForumPages.team),
                    false,
                    false,
                    null,
                    null);
            }

            // Help
            if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ShowHelpTo))
            {
                RenderMenuItem(
                    this.menuListItems,
                    "<i class=\"fa fa-question fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("TOOLBAR", "HELP")),
                    this.GetText("TOOLBAR", "HELP_TITLE"),
                    YafBuildLink.GetLink(ForumPages.help_index),
                    false,
                    false,
                    null,
                    null);
            }
        }

        /// <summary>
        /// Render The User related Links
        /// </summary>
        private void RenderUserContainer()
        {
            RenderMenuItem(
                    this.MyProfile,
                    "<i class=\"fa fa-user fa-fw\"></i>  {0}".FormatWith(this.GetText("TOOLBAR", "MYPROFILE")),
                    this.GetText("TOOLBAR", "MYPROFILE_TITLE"),
                    YafBuildLink.GetLink(ForumPages.cp_profile),
                    false,
                    this.PageContext.UnreadPrivate > 0,
                    this.PageContext.UnreadPrivate.ToString(),
                    this.GetText("TOOLBAR", "NEWPM").FormatWith(this.PageContext.UnreadPrivate));

            // My Inbox
            if (this.Get<YafBoardSettings>().AllowPrivateMessages)
            {
                RenderMenuItem(
                    this.MyInboxItem,
                    "<i class=\"fa fa-envelope fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("TOOLBAR", "INBOX")),
                    this.GetText("TOOLBAR", "INBOX_TITLE"),
                    YafBuildLink.GetLink(ForumPages.cp_pm),
                    false,
                    false,
                    null,
                    null);
            }

            // My Buddies
            if (this.Get<YafBoardSettings>().EnableBuddyList && this.PageContext.UserHasBuddies)
            {
                RenderMenuItem(
                    this.MyBuddiesItem,
                     "<i class=\"fa fa-users fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("TOOLBAR", "BUDDIES")),
                    this.GetText("TOOLBAR", "BUDDIES_TITLE"),
                    YafBuildLink.GetLink(ForumPages.cp_editbuddies),
                    false,
                    this.PageContext.PendingBuddies > 0,
                    this.PageContext.PendingBuddies.ToString(),
                    this.GetText("TOOLBAR", "BUDDYREQUEST").FormatWith(this.PageContext.PendingBuddies));
            }

            // My Albums
            if (this.Get<YafBoardSettings>().EnableAlbum && (this.PageContext.UsrAlbums > 0 || this.PageContext.NumAlbums > 0))
            {
                RenderMenuItem(
                    this.MyAlbumsItem,
                     "<i class=\"fa fa-image fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("TOOLBAR", "MYALBUMS")),
                    this.GetText("TOOLBAR", "MYALBUMS_TITLE"),
                    YafBuildLink.GetLinkNotEscaped(ForumPages.albums, "u={0}", this.PageContext.PageUserID),
                    false,
                    false,
                    null,
                    null);
            }

            // My Topics
            RenderMenuItem(
                this.MyTopicItem,
                 "<i class=\"fa fa-comment fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("TOOLBAR", "MYTOPICS")),
                this.GetText("TOOLBAR", "MYTOPICS"),
                YafBuildLink.GetLink(ForumPages.mytopics),
                false,
                false,
                string.Empty,
                string.Empty);

            // Logout
            if (!Config.IsAnyPortal && Config.AllowLoginAndLogoff)
            {
                this.LogoutItem.Visible = true;
                this.LogOutButton.Text = "<i class=\"fa fa-sign-out-alt fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("TOOLBAR", "LOGOUT"));
                this.LogOutButton.ToolTip = this.GetText("TOOLBAR", "LOGOUT");
            }
        }

       #endregion
    }
}