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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Model;
    using YAF.Dialogs;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;

    using ButtonStyle = YAF.Types.Constants.ButtonStyle;

    #endregion

    /// <summary>
    /// The User Menu
    /// </summary>
    public partial class UserMenu : BaseUserControl
    {
        #region Methods

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
                new DialogButton
                    {
                        Text = this.GetText("TOOLBAR", "LOGOUT"),
                        CssClass = "btn btn-primary",
                        ForumPageLink = new ForumLink { ForumPage = ForumPages.Logout }
                    },
                new DialogButton { Text = this.GetText("COMMON", "CANCEL"), CssClass = "btn btn-secondary" });
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RenderUserContainer();
        }

        /// <summary>
        /// Mark all Activity as read
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void MarkAll_Click(object sender, EventArgs e)
        {
            this.GetRepository<Activity>().MarkAllAsRead(this.PageContext.PageUserID);

            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageContext.PageUserID));

            this.Get<HttpResponseBase>().Redirect(this.Get<HttpRequestBase>().Url.ToString(), true);
        }

        /// <summary>
        /// Render Li and a Item
        /// </summary>
        /// <param name="holder">
        /// The holder.
        /// </param>
        /// <param name="cssClass">
        /// The CSS class.
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
        /// Add no follow to the link
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
        /// <param name="isActive">
        /// The is Active.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        private static void RenderMenuItem(
            Control holder,
            string cssClass,
            string linkText,
            string linkToolTip,
            string linkUrl,
            bool noFollow,
            bool showUnread,
            string unread,
            string unreadText,
            bool isActive,
            string icon = "")
        {
            var element = new HtmlGenericControl("li");

            if (cssClass.IsSet())
            {
                element.Attributes.Add("class", "nav-item");
            }

            if (isActive)
            {
                cssClass = $"{cssClass} active";
            }

            if (linkToolTip.IsNotSet())
            {
                linkToolTip = linkText;
            }

            var link = new HyperLink
                           {
                               Target = "_top",
                               ToolTip = linkToolTip,
                               NavigateUrl = linkUrl,
                               Text = icon.IsSet() ? $"<i class=\"fa fa-{icon} fa-fw\"></i>&nbsp;{linkText}" : linkText,
                               CssClass = cssClass
                           };

            if (noFollow)
            {
                link.Attributes.Add("rel", "nofollow");
            }

            link.Attributes.Add("data-toggle", "tooltip");

            if (showUnread)
            {
                link.Controls.Add(
                    new LiteralControl(
                        icon.IsSet()
                            ? $"<i class=\"fa fa-{icon} fa-fw\"></i>&nbsp;{linkText}&nbsp;"
                            : $"{linkText}&nbsp;"));

                var unreadLabel = new Label { CssClass = "badge badge-danger", ToolTip = unreadText, Text = unread };

                unreadLabel.Attributes.Add("data-toggle", "tooltip");

                var unreadLabelText = new Label { CssClass = "sr-only", Text = unreadText };

                link.Controls.Add(unreadLabel);

                link.Controls.Add(unreadLabelText);
            }

            if (cssClass.Equals("nav-link"))
            {
                element.Controls.Add(link);
                holder.Controls.Add(element);
            }
            else
            {
                holder.Controls.Add(link);
            }
        }

        /// <summary>
        /// Render The User related Links
        /// </summary>
        private void RenderUserContainer()
        {
            if (this.PageContext.IsGuest)
            {
                return;
            }

            RenderMenuItem(
                this.MyProfile,
                "dropdown-item",
                this.GetText("TOOLBAR", "MYPROFILE"),
                this.GetText("TOOLBAR", "MYPROFILE_TITLE"),
                BuildLink.GetLink(ForumPages.Account),
                false,
                false,
                null,
                null,
                this.PageContext.ForumPageType == ForumPages.Account,
                "address-card");

            if (!Config.IsDotNetNuke)
            {
                RenderMenuItem(
                    this.MySettings,
                    "dropdown-item",
                    this.GetText("EDIT_PROFILE"),
                    this.GetText("EDIT_PROFILE"),
                    BuildLink.GetLink(ForumPages.EditProfile),
                    false,
                    false,
                    null,
                    null,
                    this.PageContext.ForumPageType == ForumPages.EditProfile,
                    "user-edit");

                RenderMenuItem(
                    this.MySettings,
                    "dropdown-item",
                    this.GetText("ACCOUNT", "EDIT_SETTINGS"),
                    this.GetText("ACCOUNT", "EDIT_SETTINGS"),
                    BuildLink.GetLink(ForumPages.EditSettings),
                    false,
                    false,
                    null,
                    null,
                    this.PageContext.ForumPageType == ForumPages.EditSettings,
                    "user-cog");
            }

            RenderMenuItem(
                this.MySettings,
                "dropdown-item",
                this.GetText("ATTACHMENTS", "TITLE"),
                this.GetText("ATTACHMENTS", "TITLE"),
                BuildLink.GetLink(ForumPages.Attachments),
                false,
                false,
                null,
                null,
                this.PageContext.ForumPageType == ForumPages.Attachments,
                "paperclip");

            if (!Config.IsDotNetNuke && (this.Get<BoardSettings>().AvatarRemote
                                         || this.Get<BoardSettings>().AvatarUpload
                                         || this.Get<BoardSettings>().AvatarGallery
                                         || this.Get<BoardSettings>().AvatarGravatar))
            {
                RenderMenuItem(
                    this.MySettings,
                    "dropdown-item",
                    this.GetText("ACCOUNT", "EDIT_AVATAR"),
                    this.GetText("ACCOUNT", "EDIT_AVATAR"),
                    BuildLink.GetLink(ForumPages.EditAvatar),
                    false,
                    false,
                    null,
                    null,
                    this.PageContext.ForumPageType == ForumPages.EditAvatar,
                    "user-tie");
            }

            if (this.Get<BoardSettings>().AllowSignatures)
            {
                RenderMenuItem(
                    this.MySettings,
                    "dropdown-item",
                    this.GetText("ACCOUNT", "SIGNATURE"),
                    this.GetText("ACCOUNT", "SIGNATURE"),
                    BuildLink.GetLink(ForumPages.EditSignature),
                    false,
                    false,
                    null,
                    null,
                    this.PageContext.ForumPageType == ForumPages.EditSignature,
                    "signature");
            }

            RenderMenuItem(
                this.MySettings,
                "dropdown-item",
                this.GetText("ACCOUNT", "SUBSCRIPTIONS"),
                this.GetText("ACCOUNT", "SUBSCRIPTIONS"),
                BuildLink.GetLink(ForumPages.Subscriptions),
                false,
                false,
                null,
                null,
                this.PageContext.ForumPageType == ForumPages.Subscriptions,
                "envelope");

            RenderMenuItem(
                this.MySettings,
                "dropdown-item",
                this.GetText("BLOCK_OPTIONS", "TITLE"),
                this.GetText("BLOCK_OPTIONS", "TITLE"),
                BuildLink.GetLink(ForumPages.BlockOptions),
                false,
                false,
                null,
                null,
                this.PageContext.ForumPageType == ForumPages.BlockOptions,
                "user-lock");

            if (!Config.IsDotNetNuke && this.Get<BoardSettings>().AllowPasswordChange)
            {
                // Render Change Password Item
                RenderMenuItem(
                    this.MySettings,
                    "dropdown-item",
                    this.GetText("ACCOUNT", "CHANGE_PASSWORD"),
                    this.GetText("ACCOUNT", "CHANGE_PASSWORD"),
                    BuildLink.GetLink(ForumPages.ChangePassword),
                    false,
                    false,
                    null,
                    null,
                    this.PageContext.ForumPageType == ForumPages.ChangePassword,
                    "lock");
            }

            if (!Config.IsDotNetNuke && !this.PageContext.IsAdmin && !this.PageContext.IsHostAdmin)
            {
                // Render Delete Account Item
                RenderMenuItem(
                    this.MySettings,
                    "dropdown-item",
                    this.GetText("ACCOUNT", "DELETE_ACCOUNT"),
                    this.GetText("ACCOUNT", "DELETE_ACCOUNT"),
                    BuildLink.GetLink(ForumPages.DeleteAccount),
                    false,
                    false,
                    null,
                    null,
                    this.PageContext.ForumPageType == ForumPages.DeleteAccount,
                    "user-alt-slash");
            }

            // My Inbox
            if (this.Get<BoardSettings>().AllowPrivateMessages)
            {
                RenderMenuItem(
                    this.MyInboxItem,
                    "dropdown-item",
                    this.GetText("TOOLBAR", "INBOX"),
                    this.GetText("TOOLBAR", "INBOX_TITLE"),
                    BuildLink.GetLink(ForumPages.PM),
                    false,
                    this.PageContext.UnreadPrivate > 0,
                    this.PageContext.UnreadPrivate.ToString(),
                    this.GetTextFormatted("NEWPM", this.PageContext.UnreadPrivate),
                    this.PageContext.ForumPageType == ForumPages.PM,
                    "inbox");
            }

            // My Buddies
            if (this.Get<BoardSettings>().EnableBuddyList && this.PageContext.UserHasBuddies)
            {
                RenderMenuItem(
                    this.MyBuddiesItem,
                    "dropdown-item",
                    this.GetText("TOOLBAR", "BUDDIES"),
                    this.GetText("TOOLBAR", "BUDDIES_TITLE"),
                    BuildLink.GetLink(ForumPages.Friends),
                    false,
                    this.PageContext.PendingBuddies > 0,
                    this.PageContext.PendingBuddies.ToString(),
                    this.GetTextFormatted("BUDDYREQUEST", this.PageContext.PendingBuddies),
                    this.PageContext.ForumPageType == ForumPages.Friends,
                    "users");
            }

            // My Albums
            if (this.Get<BoardSettings>().EnableAlbum
                && (this.PageContext.UsrAlbums > 0 || this.PageContext.NumAlbums > 0))
            {
                RenderMenuItem(
                    this.MyAlbumsItem,
                    "dropdown-item",
                    this.GetText("TOOLBAR", "MYALBUMS"),
                    this.GetText("TOOLBAR", "MYALBUMS_TITLE"),
                    BuildLink.GetLinkNotEscaped(ForumPages.Albums, "u={0}", this.PageContext.PageUserID),
                    false,
                    false,
                    null,
                    null,
                    this.PageContext.ForumPageType == ForumPages.Albums,
                    "images");
            }

            // My Topics
            RenderMenuItem(
                this.MyTopicItem,
                "dropdown-item",
                this.GetText("TOOLBAR", "MYTOPICS"),
                this.GetText("TOOLBAR", "MYTOPICS"),
                BuildLink.GetLink(ForumPages.MyTopics),
                false,
                false,
                string.Empty,
                string.Empty,
                this.PageContext.ForumPageType == ForumPages.MyTopics,
                "comment");

            // Logout
            if (!Config.IsAnyPortal && Config.AllowLoginAndLogoff)
            {
                this.LogutItem.Visible = true;
                this.LogOutButton.Text =
                    $"<i class=\"fa fa-sign-out-alt fa-fw\"></i>&nbsp;{this.GetText("TOOLBAR", "LOGOUT")}";
                this.LogOutButton.ToolTip = this.GetText("TOOLBAR", "LOGOUT");
            }

            this.UserAvatar.ImageUrl = this.Get<IAvatars>().GetAvatarUrlForCurrentUser();

            this.UserDropDown.DataToggle = "dropdown";
            this.UserDropDown.Type = ButtonStyle.None;

            if (this.PageContext.ForumPageType == ForumPages.Account
                || this.PageContext.ForumPageType == ForumPages.EditProfile
                || this.PageContext.ForumPageType == ForumPages.PM
                || this.PageContext.ForumPageType == ForumPages.Friends
                || this.PageContext.ForumPageType == ForumPages.MyTopics
                || this.PageContext.ForumPageType == ForumPages.EditProfile
                || this.PageContext.ForumPageType == ForumPages.EditSettings
                || this.PageContext.ForumPageType == ForumPages.ChangePassword
                || this.PageContext.ForumPageType == ForumPages.Attachments
                || this.PageContext.ForumPageType == ForumPages.Avatar
                || this.PageContext.ForumPageType == ForumPages.EditAvatar
                || this.PageContext.ForumPageType == ForumPages.EditSignature
                || this.PageContext.ForumPageType == ForumPages.Subscriptions
                || this.PageContext.ForumPageType == ForumPages.BlockOptions)
            {
                this.UserDropDown.CssClass = "nav-link active dropdown-toggle";
            }
            else
            {
                this.UserDropDown.CssClass = "nav-link dropdown-toggle";
            }

            this.UserDropDown.NavigateUrl = BuildLink.GetLink(
                ForumPages.Profile,
                "u={0}&name={1}",
                this.PageContext.PageUserID,
                this.Get<BoardSettings>().EnableDisplayName
                    ? this.PageContext.CurrentUserData.DisplayName
                    : this.PageContext.CurrentUserData.UserName);

            var unreadCount = this.PageContext.UnreadPrivate + this.PageContext.PendingBuddies;

            var unreadNotify = this.PageContext.Mention + this.PageContext.Quoted + this.PageContext.ReceivedThanks;

            if (!this.PageContext.CurrentUserData.Activity)
            {
                this.MyNotifications.Visible = false;
            }
            else
            {
                if (unreadNotify == 0)
                {
                    this.NotifyPopMenu.Visible = false;
                    this.NotifyIcon.IconType = string.Empty;

                    this.UnreadIcon.Visible = false;

                    this.NotifyItem.DataToggle = "tooltip";
                    this.NotifyItem.CssClass = "nav-link mb-1";
                    this.NotifyItem.NavigateUrl = BuildLink.GetLink(ForumPages.Notification);
                }
            }

            if (unreadCount <= 0)
            {
                this.UnreadPlaceHolder.Visible = false;
                return;
            }

            this.UnreadLabel.Text = unreadCount.ToString();

            this.UnreadPlaceHolder.Visible = true;
        }

        #endregion
    }
}