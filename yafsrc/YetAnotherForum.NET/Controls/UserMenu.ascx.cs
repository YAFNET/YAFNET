/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Controls;

using System.Web.UI;
using System.Web.UI.HtmlControls;

using YAF.Types.EventProxies;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models;

/// <summary>
/// The User Menu
/// </summary>
public partial class UserMenu : BaseUserControl
{
    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.PreRender" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        if (!this.PageBoardContext.IsGuest)
        {
            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                nameof(JavaScriptBlocks.LogOutJs),
                JavaScriptBlocks.LogOutJs(
                    this.GetText("TOOLBAR", "LOGOUT_TITLE"),
                    this.GetText("TOOLBAR", "LOGOUT_QUESTION"),
                    this.GetText("TOOLBAR", "LOGOUT"),
                    this.GetText("COMMON", "CANCEL"),
                    this.Get<LinkBuilder>().GetLink(ForumPages.Account_Logout)));
        }

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
        this.GetRepository<Activity>().MarkAllAsRead(this.PageBoardContext.PageUserID);

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageBoardContext.PageUserID));

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
                           ToolTip = linkToolTip,
                           NavigateUrl = linkUrl,
                           Text = icon.IsSet() ? $"<i class=\"fa fa-{icon} fa-fw\"></i>&nbsp;{linkText}" : linkText,
                           CssClass = cssClass
                       };

        if (noFollow)
        {
            link.Attributes.Add("rel", "nofollow");
        }

        link.Attributes.Add("data-bs-toggle", "tooltip");

        if (showUnread)
        {
            link.Controls.Add(
                new LiteralControl(
                    icon.IsSet()
                        ? $"<i class=\"fa fa-{icon} fa-fw\"></i>&nbsp;{linkText}&nbsp;"
                        : $"{linkText}&nbsp;"));

            var unreadLabel = new Label { CssClass = "badge text-bg-danger", ToolTip = unreadText, Text = unread };

            unreadLabel.Attributes.Add("data-bs-toggle", "tooltip");

            var unreadLabelText = new Label { CssClass = "visually-hidden", Text = unreadText };

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
        if (this.PageBoardContext.IsGuest)
        {
            return;
        }

        RenderMenuItem(
            this.MyProfile,
            "dropdown-item",
            this.GetText("TOOLBAR", "MYPROFILE"),
            this.GetText("TOOLBAR", "MYPROFILE_TITLE"),
            this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount),
            false,
            false,
            null,
            null,
            this.PageBoardContext.CurrentForumPage.PageType == ForumPages.MyAccount,
            "address-card");

        if (!Config.IsDotNetNuke)
        {
            RenderMenuItem(
                this.MySettings,
                "dropdown-item",
                this.GetText("EDIT_PROFILE"),
                this.GetText("EDIT_PROFILE"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Profile_EditProfile),
                false,
                false,
                null,
                null,
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Profile_EditProfile,
                "user-edit");

            RenderMenuItem(
                this.MySettings,
                "dropdown-item",
                this.GetText("ACCOUNT", "EDIT_SETTINGS"),
                this.GetText("ACCOUNT", "EDIT_SETTINGS"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Profile_EditSettings),
                false,
                false,
                null,
                null,
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Profile_EditSettings,
                "user-cog");
        }

        RenderMenuItem(
            this.MySettings,
            "dropdown-item",
            this.GetText("ATTACHMENTS", "TITLE"),
            this.GetText("ATTACHMENTS", "TITLE"),
            this.Get<LinkBuilder>().GetLink(ForumPages.Profile_Attachments),
            false,
            false,
            null,
            null,
            this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Profile_Attachments,
            "paperclip");

        if (!Config.IsDotNetNuke && (this.PageBoardContext.BoardSettings.AvatarUpload
                                     || this.PageBoardContext.BoardSettings.AvatarGallery))
        {
            RenderMenuItem(
                this.MySettings,
                "dropdown-item",
                this.GetText("ACCOUNT", "EDIT_AVATAR"),
                this.GetText("ACCOUNT", "EDIT_AVATAR"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Profile_EditAvatar),
                false,
                false,
                null,
                null,
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Profile_EditAvatar,
                "user-tie");
        }

        if (this.PageBoardContext.BoardSettings.AllowSignatures)
        {
            RenderMenuItem(
                this.MySettings,
                "dropdown-item",
                this.GetText("ACCOUNT", "SIGNATURE"),
                this.GetText("ACCOUNT", "SIGNATURE"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Profile_EditSignature),
                false,
                false,
                null,
                null,
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Profile_EditSignature,
                "signature");
        }

        RenderMenuItem(
            this.MySettings,
            "dropdown-item",
            this.GetText("ACCOUNT", "SUBSCRIPTIONS"),
            this.GetText("ACCOUNT", "SUBSCRIPTIONS"),
            this.Get<LinkBuilder>().GetLink(ForumPages.Profile_Subscriptions),
            false,
            false,
            null,
            null,
            this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Profile_Subscriptions,
            "envelope");

        RenderMenuItem(
            this.MySettings,
            "dropdown-item",
            this.GetText("BLOCK_OPTIONS", "TITLE"),
            this.GetText("BLOCK_OPTIONS", "TITLE"),
            this.Get<LinkBuilder>().GetLink(ForumPages.Profile_BlockOptions),
            false,
            false,
            null,
            null,
            this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Profile_BlockOptions,
            "user-lock");

        if (!Config.IsDotNetNuke)
        {
            // Render Change Password Item
            RenderMenuItem(
                this.MySettings,
                "dropdown-item",
                this.GetText("ACCOUNT", "CHANGE_PASSWORD"),
                this.GetText("ACCOUNT", "CHANGE_PASSWORD"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Profile_ChangePassword),
                false,
                false,
                null,
                null,
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Profile_ChangePassword,
                "lock");
        }

        if (!Config.IsDotNetNuke && !this.PageBoardContext.PageUser.UserFlags.IsHostAdmin)
        {
            // Render Delete Account Item
            RenderMenuItem(
                this.MySettings,
                "dropdown-item",
                this.GetText("ACCOUNT", "DELETE_ACCOUNT"),
                this.GetText("ACCOUNT", "DELETE_ACCOUNT"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Profile_DeleteAccount),
                false,
                false,
                null,
                null,
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Profile_DeleteAccount,
                "user-alt-slash");
        }

        // My Inbox
        if (this.PageBoardContext.BoardSettings.AllowPrivateMessages)
        {
            RenderMenuItem(
                this.MyInboxItem,
                "dropdown-item",
                this.GetText("TOOLBAR", "INBOX"),
                this.GetText("TOOLBAR", "INBOX_TITLE"),
                this.Get<LinkBuilder>().GetLink(ForumPages.MyMessages),
                false,
                this.PageBoardContext.UnreadPrivate > 0,
                this.PageBoardContext.UnreadPrivate.ToString(),
                this.GetTextFormatted("NEWPM", this.PageBoardContext.UnreadPrivate),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.MyMessages,
                "inbox");
        }

        // My Buddies
        if (this.PageBoardContext.BoardSettings.EnableBuddyList && this.PageBoardContext.UserHasBuddies)
        {
            RenderMenuItem(
                this.MyBuddiesItem,
                "dropdown-item",
                this.GetText("TOOLBAR", "BUDDIES"),
                this.GetText("TOOLBAR", "BUDDIES_TITLE"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Friends),
                false,
                this.PageBoardContext.PendingBuddies > 0,
                this.PageBoardContext.PendingBuddies.ToString(),
                this.GetTextFormatted("BUDDYREQUEST", this.PageBoardContext.PendingBuddies),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Friends,
                "users");
        }

        // My Albums
        if (this.PageBoardContext.BoardSettings.EnableAlbum
            && this.PageBoardContext.NumAlbums > 0)
        {
            RenderMenuItem(
                this.MyAlbumsItem,
                "dropdown-item",
                this.GetText("TOOLBAR", "MYALBUMS"),
                this.GetText("TOOLBAR", "MYALBUMS_TITLE"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Albums, new {u = this.PageBoardContext.PageUserID }),
                false,
                false,
                null,
                null,
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Albums,
                "images");
        }

        // My Topics
        RenderMenuItem(
            this.MyTopicItem,
            "dropdown-item",
            this.GetText("TOOLBAR", "MYTOPICS"),
            this.GetText("TOOLBAR", "MYTOPICS"),
            this.Get<LinkBuilder>().GetLink(ForumPages.MyTopics),
            false,
            false,
            string.Empty,
            string.Empty,
            this.PageBoardContext.CurrentForumPage.PageType == ForumPages.MyTopics,
            "comment");

        // Logout
        if (!Config.IsAnyPortal && Config.AllowLoginAndLogoff)
        {
            this.LogutItem.Visible = true;
            this.LogOutButton.NavigateUrl = "javascript:LogOutClick()";
        }

        this.UserAvatar.ImageUrl = this.Get<IAvatars>().GetAvatarUrlForCurrentUser();

        this.UserDropDown.DataToggle = "dropdown";
        this.UserDropDown.Type = ButtonStyle.None;

        this.UserDropDown.CssClass =
            this.PageBoardContext.CurrentForumPage.PageType is ForumPages.MyAccount or ForumPages.Profile_EditProfile or
                ForumPages.MyMessages or ForumPages.Friends or ForumPages.MyTopics or ForumPages.Profile_EditProfile
                or ForumPages.Profile_EditSettings or ForumPages.Profile_ChangePassword or
                ForumPages.Profile_Attachments or ForumPages.Profile_EditAvatar or ForumPages.Profile_EditSignature
                or ForumPages.Profile_Subscriptions or ForumPages.Profile_BlockOptions
                ? "nav-link active dropdown-toggle"
                : "nav-link dropdown-toggle";

        this.UserDropDown.NavigateUrl = "#";

        var unreadCount = this.PageBoardContext.UnreadPrivate + this.PageBoardContext.PendingBuddies;

        var unreadNotify = this.PageBoardContext.Mention + this.PageBoardContext.Quoted + this.PageBoardContext.ReceivedThanks + this.PageBoardContext.WatchTopic;

        if (!this.PageBoardContext.PageUser.Activity)
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
                this.NotifyItem.CssClass = "nav-link";
                this.NotifyItem.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.Notification);
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
}