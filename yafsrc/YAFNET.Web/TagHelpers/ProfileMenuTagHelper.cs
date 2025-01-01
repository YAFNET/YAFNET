/* Yet Another Forum.NET
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

namespace YAF.Web.TagHelpers;

/// <summary>
///  PageUser Profile Menu in the PageUser Control Panel
/// </summary>
[HtmlTargetElement("profileMenu")]
public class ProfileMenuTagHelper : TagHelper, IHaveServiceLocator, IHaveLocalization
{
    /// <summary>
    ///   The localization.
    /// </summary>
    private ILocalization localization;

    /// <summary>
    ///   Gets Localization.
    /// </summary>
    public ILocalization Localization => this.localization ??= this.Get<ILocalization>();

    /// <summary>
    ///   Gets PageContext.
    /// </summary>
    public BoardContext PageContext => BoardContext.Current;

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => this.PageContext.ServiceLocator;

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="output">
    /// The output.
    /// </param>
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var html = new TagBuilder(HtmlTag.Div);
        var htmlDropDown = new TagBuilder(HtmlTag.Div);

        htmlDropDown.AddCssClass("dropdown d-md-none d-grid gap-2 mb-3");

        var dropDownButton = new TagBuilder(HtmlTag.Button);

        dropDownButton.AddCssClass("btn btn-secondary dropdown-toggle");

        dropDownButton.MergeAttribute(HtmlAttribute.Id, "dropdownMenuButton");
        dropDownButton.MergeAttribute(HtmlAttribute.Type, HtmlTag.Button);
        dropDownButton.MergeAttribute("data-bs-toggle", "dropdown");
        dropDownButton.MergeAttribute("aria-haspopup", "true");
        dropDownButton.MergeAttribute(HtmlAttribute.AriaExpanded, "false");

        var icon = new TagBuilder(HtmlTag.I);
        icon.AddCssClass("fa fa-cogs fa-fw me-1");

        dropDownButton.InnerHtml.AppendHtml(icon);
        dropDownButton.InnerHtml.Append(this.GetText("CONTROL_PANEL"));

        htmlDropDown.InnerHtml.AppendHtml(dropDownButton);

        var htmlDropDownMenu = new TagBuilder(HtmlTag.Div);
        htmlDropDownMenu.AddCssClass("dropdown-menu scrollable-dropdown");
        htmlDropDownMenu.MergeAttribute("aria-labelledby", "dropdownMenuButton");

        html.AddCssClass("list-group d-none d-md-block");

        html.InnerHtml.AppendHtml(this.RenderMenuItem(
            "list-group-item list-group-item-action",
            ForumPages.MyAccount,
            this.GetText("YOUR_ACCOUNT"),
            "address-card"));

        this.RenderMenuItem("dropdown-item", ForumPages.MyAccount, this.GetText("YOUR_ACCOUNT"), "address-card");

        html.InnerHtml.AppendHtml(this.RenderMenuItem(
            "list-group-item list-group-item-action",
            ForumPages.UserProfile,
            this.GetText("VIEW_PROFILE"),
            "user",
            new { u = this.PageContext.PageUserID, name = this.PageContext.MembershipUser.UserName }));

        var headerProfile = new TagBuilder(HtmlTag.H6);
        headerProfile.AddCssClass("dropdown-header");
        headerProfile.InnerHtml.Append(this.GetText("PERSONAL_PROFILE"));

        htmlDropDownMenu.InnerHtml.AppendHtml(headerProfile);

        htmlDropDownMenu.InnerHtml.AppendHtml(this.RenderMenuItem(
            "dropdown-item",
            ForumPages.UserProfile,
            this.GetText("VIEW_PROFILE"),
            "user",
            new {u = this.PageContext.PageUserID , name = this.PageContext.MembershipUser.UserName }));

        html.InnerHtml.AppendHtml(this.RenderMenuItem(
            "list-group-item list-group-item-action",
            ForumPages.Profile_EditProfile,
            this.GetText("EDIT_PROFILE"),
            "user-edit"));

        htmlDropDownMenu.InnerHtml.AppendHtml(this.RenderMenuItem(
            "dropdown-item",
            ForumPages.Profile_EditProfile,
            this.GetText("EDIT_PROFILE"),
            "user-edit"));

        html.InnerHtml.AppendHtml(this.RenderMenuItem(
            "list-group-item list-group-item-action",
            ForumPages.Profile_EditSettings,
            this.GetText("ACCOUNT", "EDIT_SETTINGS"),
            "user-cog"));

        htmlDropDownMenu.InnerHtml.AppendHtml(this.RenderMenuItem(
            "dropdown-item",
            ForumPages.Profile_EditSettings,
            this.GetText("ACCOUNT", "EDIT_SETTINGS"),
            "user-cog"));

        if (!this.PageContext.IsGuest && this.PageContext.UploadAccess && this.GetRepository<Attachment>()
                .Exists(x => x.UserID == this.PageContext.PageUserID))
        {
            html.InnerHtml.AppendHtml(this.RenderMenuItem(
                "list-group-item list-group-item-action",
                ForumPages.Profile_Attachments,
                this.GetText("ATTACHMENTS", "TITLE"),
                "paperclip"));

            htmlDropDownMenu.InnerHtml.AppendHtml(this.RenderMenuItem(
                "dropdown-item",
                ForumPages.Profile_Attachments,
                this.GetText("ATTACHMENTS", "TITLE"),
                "paperclip"));
        }

        if (!this.PageContext.IsGuest
            && this.PageContext.BoardSettings.EnableBuddyList && this.PageContext.UserHasBuddies)
        {
            html.InnerHtml.AppendHtml(this.RenderMenuItem(
                "list-group-item list-group-item-action",
                ForumPages.Friends,
                this.GetText("EDIT_BUDDIES"),
                "users"));

            htmlDropDownMenu.InnerHtml.AppendHtml(this.RenderMenuItem(
                "dropdown-item",
                ForumPages.Friends,
                this.GetText("EDIT_BUDDIES"),
                "users"));
        }

        if (!this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableAlbum)
        {
            html.InnerHtml.AppendHtml(
                this.RenderMenuItem(
                    "list-group-item list-group-item-action",
                    ForumPages.Albums,
                    this.GetText("EDIT_ALBUMS"),
                    "images",
                    new { u = this.PageContext.PageUserID }));

            htmlDropDownMenu.InnerHtml.AppendHtml(this.RenderMenuItem(
                "dropdown-item",
                ForumPages.Albums,
                this.GetText("EDIT_ALBUMS"),
                "images",
                new { u = this.PageContext.PageUserID}));
        }

        if (this.PageContext.BoardSettings.AvatarUpload
            || this.PageContext.BoardSettings.AvatarGallery)
        {
            html.InnerHtml.AppendHtml(this.RenderMenuItem(
                "list-group-item list-group-item-action",
                ForumPages.Profile_EditAvatar,
                this.GetText("ACCOUNT", "EDIT_AVATAR"),
                "user-tie"));

            htmlDropDownMenu.InnerHtml.AppendHtml(this.RenderMenuItem(
                "dropdown-item",
                ForumPages.Profile_EditAvatar,
                this.GetText("ACCOUNT", "EDIT_AVATAR"),
                "user-tie"));
        }

        if (this.PageContext.BoardSettings.AllowSignatures)
        {
            html.InnerHtml.AppendHtml(this.RenderMenuItem(
                "list-group-item list-group-item-action",
                ForumPages.Profile_EditSignature,
                this.GetText("ACCOUNT", "SIGNATURE"),
                "signature"));

            htmlDropDownMenu.InnerHtml.AppendHtml(
                this.RenderMenuItem(
                    "dropdown-item",
                    ForumPages.Profile_EditSignature,
                    this.GetText("ACCOUNT", "SIGNATURE"),
                    "signature"));
        }

        html.InnerHtml.AppendHtml(this.RenderMenuItem(
            "list-group-item list-group-item-action",
            ForumPages.Profile_Subscriptions,
            this.GetText("ACCOUNT", "SUBSCRIPTIONS"),
            "envelope"));

        htmlDropDownMenu.InnerHtml.AppendHtml(this.RenderMenuItem(
            "dropdown-item",
            ForumPages.Profile_Subscriptions,
            this.GetText("ACCOUNT", "SUBSCRIPTIONS"),
            "envelope"));

        html.InnerHtml.AppendHtml(this.RenderMenuItem(
            "list-group-item list-group-item-action",
            ForumPages.Profile_BlockOptions,
            this.GetText("BLOCK_OPTIONS", "TITLE"),
            "user-lock"));

        htmlDropDownMenu.InnerHtml.AppendHtml(this.RenderMenuItem(
            "dropdown-item",
            ForumPages.Profile_BlockOptions,
            this.GetText("BLOCK_OPTIONS", "TITLE"),
            "user-lock"));

        // Render Change Password Item
        html.InnerHtml.AppendHtml(this.RenderMenuItem(
            "list-group-item list-group-item-action",
            ForumPages.Profile_ChangePassword,
            this.GetText("ACCOUNT", "CHANGE_PASSWORD"),
            "lock"));

        htmlDropDownMenu.InnerHtml.AppendHtml(this.RenderMenuItem(
            "dropdown-item",
            ForumPages.Profile_ChangePassword,
            this.GetText("ACCOUNT", "CHANGE_PASSWORD"),
            "lock"));

        if (!this.PageContext.PageUser.UserFlags.IsHostAdmin)
        {
            // Render Delete Account Item
            html.InnerHtml.AppendHtml(this.RenderMenuItem(
                "list-group-item list-group-item-action",
                ForumPages.Profile_DeleteAccount,
                this.GetText("ACCOUNT", "DELETE_ACCOUNT"),
                "user-alt-slash"));

            htmlDropDownMenu.InnerHtml.AppendHtml(this.RenderMenuItem(
                "dropdown-item",
                ForumPages.Profile_DeleteAccount,
                this.GetText("ACCOUNT", "DELETE_ACCOUNT"),
                "user-alt-slash"));
        }

        htmlDropDown.InnerHtml.AppendHtml(htmlDropDownMenu);

        output.Content.AppendHtml(html);
        output.Content.AppendHtml(htmlDropDown);
        return Task.CompletedTask;
    }

    /// <summary>
    /// The render menu item.
    /// </summary>
    /// <param name="cssClass">
    /// The CSS class.
    /// </param>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="getText">
    /// The get text.
    /// </param>
    /// <param name="iconName">
    /// The icon name.
    /// </param>
    /// <param name="parameter">
    /// The URL Parameter
    /// </param>
    private IHtmlContent RenderMenuItem(
        string cssClass,
        ForumPages page,
        string getText,
        string iconName,
        object parameter = null)
    {
        var link = new TagBuilder(HtmlTag.A);

        link.AddCssClass(BoardContext.Current.CurrentForumPage.PageName == page ? $"{cssClass} active" : cssClass);

        link.MergeAttribute(
            HtmlAttribute.Href,
            parameter is not null
                ? this.Get<LinkBuilder>().GetLink(page, parameter)
                : this.Get<LinkBuilder>().GetLink(page));

        link.MergeAttribute("data-bs-toggle", "tooltip");
        link.MergeAttribute(HtmlAttribute.Title, getText);

        var icon = new TagBuilder(HtmlTag.I);

        icon.AddCssClass(
            $"fas fa-{iconName} me-1 {(BoardContext.Current.CurrentForumPage.PageName == page ? "text-light" : "text-secondary")}");

        link.InnerHtml.AppendHtml(icon);

        link.InnerHtml.Append(getText);

        return link;
    }
}