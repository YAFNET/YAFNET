﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Web.Controls;

/// <summary>
/// User Profile Menu in the User Control Panel
/// </summary>
public class ProfileMenu : BaseControl
{
    /// <summary>
    /// Render the Profile Menu
    /// </summary>
    /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
    override protected void Render(HtmlTextWriter writer)
    {
        var html = new StringBuilder();
        var htmlDropDown = new StringBuilder();

        htmlDropDown.Append(@"<div class=""dropdown d-md-none d-grid gap-2 mb-3"">");

        htmlDropDown.Append(
            @"<button class=""btn btn-secondary dropdown-toggle"" type=""button"" id=""dropdownMenuButton"" data-bs-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">");
        htmlDropDown.AppendFormat(@"<i class=""fa fa-cogs fa-fw""></i>&nbsp;{0}", this.GetText("CONTROL_PANEL"));
        htmlDropDown.Append(@"</button>");

        htmlDropDown.Append(
            @"<div class=""dropdown-menu scrollable-dropdown"" aria-labelledby=""dropdownMenuButton"">");

        html.Append(@"<div class=""list-group d-none d-md-block"">");

        this.RenderMenuItem(
            html,
            "list-group-item list-group-item-action",
            ForumPages.MyAccount,
            this.GetText("YOUR_ACCOUNT"),
            "address-card");

        this.RenderMenuItem(htmlDropDown, "dropdown-item", ForumPages.MyAccount, this.GetText("YOUR_ACCOUNT"), "address-card");

        this.RenderMenuItem(
            html,
            "list-group-item list-group-item-action",
            ForumPages.UserProfile,
            this.GetText("VIEW_PROFILE"),
            "user",
            new { u = this.PageBoardContext.PageUserID, name = this.PageBoardContext.PageUser.DisplayOrUserName() });

        htmlDropDown.AppendFormat(@"<h6 class=""dropdown-header"">{0}</h6>", this.GetText("PERSONAL_PROFILE"));

        this.RenderMenuItem(
            htmlDropDown,
            "dropdown-item",
            ForumPages.UserProfile,
            this.GetText("VIEW_PROFILE"),
            "user",
            new { u = this.PageBoardContext.PageUserID, name = this.PageBoardContext.PageUser.DisplayOrUserName() });

        if (!Config.IsDotNetNuke)
        {
            this.RenderMenuItem(
                html,
                "list-group-item list-group-item-action",
                ForumPages.Profile_EditProfile,
                this.GetText("EDIT_PROFILE"),
                "user-edit");

            this.RenderMenuItem(
                htmlDropDown,
                "dropdown-item",
                ForumPages.Profile_EditProfile,
                this.GetText("EDIT_PROFILE"),
                "user-edit");

            this.RenderMenuItem(
                html,
                "list-group-item list-group-item-action",
                ForumPages.Profile_EditSettings,
                this.GetText("ACCOUNT", "EDIT_SETTINGS"),
                "user-cog");

            this.RenderMenuItem(
                htmlDropDown,
                "dropdown-item",
                ForumPages.Profile_EditSettings,
                this.GetText("ACCOUNT", "EDIT_SETTINGS"),
                "user-cog");
        }

        if (!this.PageBoardContext.IsGuest && this.PageBoardContext.UploadAccess && this.GetRepository<Attachment>()
                .Exists(x => x.UserID == this.PageBoardContext.PageUserID))
        {
            this.RenderMenuItem(
                html,
                "list-group-item list-group-item-action",
                ForumPages.Profile_Attachments,
                this.GetText("ATTACHMENTS", "TITLE"),
                "paperclip");

            this.RenderMenuItem(
                htmlDropDown,
                "dropdown-item",
                ForumPages.Profile_Attachments,
                this.GetText("ATTACHMENTS", "TITLE"),
                "paperclip");
        }

        if (!this.PageBoardContext.IsGuest
            && this.PageBoardContext.BoardSettings.EnableBuddyList && this.PageBoardContext.UserHasBuddies)
        {
            this.RenderMenuItem(
                html,
                "list-group-item list-group-item-action",
                ForumPages.Friends,
                this.GetText("EDIT_BUDDIES"),
                "users");

            this.RenderMenuItem(
                htmlDropDown,
                "dropdown-item",
                ForumPages.Friends,
                this.GetText("EDIT_BUDDIES"),
                "users");
        }

        if (!this.PageBoardContext.IsGuest && this.PageBoardContext.BoardSettings.EnableAlbum)
        {
            this.RenderMenuItem(
                html,
                "list-group-item list-group-item-action",
                ForumPages.Albums,
                this.GetText("EDIT_ALBUMS"),
                "images",
                new { u = this.PageBoardContext.PageUserID });

            this.RenderMenuItem(
                htmlDropDown,
                "dropdown-item",
                ForumPages.Albums,
                this.GetText("EDIT_ALBUMS"),
                "images",
                new { u = this.PageBoardContext.PageUserID });
        }

        if (!Config.IsDotNetNuke && (this.PageBoardContext.BoardSettings.AvatarUpload
                                     || this.PageBoardContext.BoardSettings.AvatarGallery))
        {
            this.RenderMenuItem(
                html,
                "list-group-item list-group-item-action",
                ForumPages.Profile_EditAvatar,
                this.GetText("ACCOUNT", "EDIT_AVATAR"),
                "user-tie");

            this.RenderMenuItem(
                htmlDropDown,
                "dropdown-item",
                ForumPages.Profile_EditAvatar,
                this.GetText("ACCOUNT", "EDIT_AVATAR"),
                "user-tie");
        }

        if (this.PageBoardContext.BoardSettings.AllowSignatures)
        {
            this.RenderMenuItem(
                html,
                "list-group-item list-group-item-action",
                ForumPages.Profile_EditSignature,
                this.GetText("ACCOUNT", "SIGNATURE"),
                "signature");

            this.RenderMenuItem(
                htmlDropDown,
                "dropdown-item",
                ForumPages.Profile_EditSignature,
                this.GetText("ACCOUNT", "SIGNATURE"),
                "signature");
        }

        this.RenderMenuItem(
            html,
            "list-group-item list-group-item-action",
            ForumPages.Profile_Subscriptions,
            this.GetText("ACCOUNT", "SUBSCRIPTIONS"),
            "envelope");

        this.RenderMenuItem(
            htmlDropDown,
            "dropdown-item",
            ForumPages.Profile_Subscriptions,
            this.GetText("ACCOUNT", "SUBSCRIPTIONS"),
            "envelope");

        this.RenderMenuItem(
            html,
            "list-group-item list-group-item-action",
            ForumPages.Profile_BlockOptions,
            this.GetText("BLOCK_OPTIONS", "TITLE"),
            "user-lock");

        this.RenderMenuItem(
            htmlDropDown,
            "dropdown-item",
            ForumPages.Profile_BlockOptions,
            this.GetText("BLOCK_OPTIONS", "TITLE"),
            "user-lock");

        if (!Config.IsDotNetNuke)
        {
            // Render Change Password Item
            this.RenderMenuItem(
                html,
                "list-group-item list-group-item-action",
                ForumPages.Profile_ChangePassword,
                this.GetText("ACCOUNT", "CHANGE_PASSWORD"),
                "lock");

            this.RenderMenuItem(
                htmlDropDown,
                "dropdown-item",
                ForumPages.Profile_ChangePassword,
                this.GetText("ACCOUNT", "CHANGE_PASSWORD"),
                "lock");
        }

        if (!Config.IsDotNetNuke && !this.PageBoardContext.PageUser.UserFlags.IsHostAdmin)
        {
            // Render Delete Account Item
            this.RenderMenuItem(
                html,
                "list-group-item list-group-item-action",
                ForumPages.Profile_DeleteAccount,
                this.GetText("ACCOUNT", "DELETE_ACCOUNT"),
                "user-alt-slash");

            this.RenderMenuItem(
                htmlDropDown,
                "dropdown-item",
                ForumPages.Profile_DeleteAccount,
                this.GetText("ACCOUNT", "DELETE_ACCOUNT"),
                "user-alt-slash");
        }

        htmlDropDown.Append("</div></div>");

        html.Append("</div>");

        writer.Write(html.ToString());
        writer.Write(htmlDropDown.ToString());
    }

    /// <summary>
    /// The render menu item.
    /// </summary>
    /// <param name="stringBuilder">
    /// The string builder.
    /// </param>
    /// <param name="cssClass">
    /// The CSS class.
    /// </param>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="getText">
    /// The get text.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="parameters">
    /// The URL Parameter
    /// </param>
    private void RenderMenuItem(
        StringBuilder stringBuilder,
        string cssClass,
        ForumPages page,
        string getText,
        string icon,
        object parameters = null)
    {
        stringBuilder.AppendFormat(
            this.PageBoardContext.CurrentForumPage.PageType == page
                ? @"<a class=""{3} active"" href=""{0}"" title=""{2}"" data-bs-toggle=""tooltip""><i class=""fas fa-{4} me-1 text-light""></i>{1}</a>"
                : @"<a class=""{3}"" href=""{0}"" title=""{2}"" data-bs-toggle=""tooltip""><i class=""fas fa-{4} me-1 text-secondary""></i>{1}</a>",
            parameters != null ? this.Get<LinkBuilder>().GetLink(page, parameters) : this.Get<LinkBuilder>().GetLink(page),
            getText,
            getText,
            cssClass,
            icon);
    }
}