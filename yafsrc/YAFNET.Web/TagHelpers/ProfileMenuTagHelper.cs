/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
    ///   Gets Localization.
    /// </summary>
    public ILocalization Localization => field ??= this.Get<ILocalization>();

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
        output.TagName = null;

        var html = new TagBuilder(HtmlTag.Div);

        html.AddCssClass("nav nav-pills flex-md-column");

        html.InnerHtml.AppendHtml(this.RenderMenuItem(
            "nav-link",
            ForumPages.MyAccount,
            this.GetText("YOUR_ACCOUNT"),
            "address-card"));

        html.InnerHtml.AppendHtml(this.RenderMenuItem(
            "nav-link",
            ForumPages.UserProfile,
            this.GetText("VIEW_PROFILE"),
            "user",
            new { u = this.PageContext.PageUserID, name = this.PageContext.MembershipUser.UserName }));

        var headerProfile = new TagBuilder(HtmlTag.H6);
        headerProfile.AddCssClass("dropdown-header");
        headerProfile.InnerHtml.Append(this.GetText("PERSONAL_PROFILE"));

        html.InnerHtml.AppendHtml(this.RenderMenuItem(
            "nav-link",
            ForumPages.Profile_EditProfile,
            this.GetText("EDIT_PROFILE"),
            "user-edit"));

        html.InnerHtml.AppendHtml(this.RenderMenuItem(
            "nav-link",
            ForumPages.Profile_EditSettings,
            this.GetText("ACCOUNT", "EDIT_SETTINGS"),
            "user-cog"));

        if (!this.PageContext.IsGuest && this.PageContext.UploadAccess && this.GetRepository<Attachment>()
                .Exists(x => x.UserID == this.PageContext.PageUserID))
        {
            html.InnerHtml.AppendHtml(this.RenderMenuItem(
                "nav-link",
                ForumPages.Profile_Attachments,
                this.GetText("ATTACHMENTS", "TITLE"),
                "paperclip"));
        }

        if (!this.PageContext.IsGuest
            && this.PageContext.BoardSettings.EnableBuddyList && this.PageContext.UserHasBuddies)
        {
            html.InnerHtml.AppendHtml(this.RenderMenuItem(
                "nav-link",
                ForumPages.MyFriends,
                this.GetText("EDIT_BUDDIES"),
                "users"));
        }

        if (!this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableAlbum)
        {
            html.InnerHtml.AppendHtml(
                this.RenderMenuItem(
                    "nav-link",
                    ForumPages.Albums,
                    this.GetText("EDIT_ALBUMS"),
                    "images",
                    new { u = this.PageContext.PageUserID }));
        }

        if (this.PageContext.BoardSettings.AvatarUpload
            || this.PageContext.BoardSettings.AvatarGallery)
        {
            html.InnerHtml.AppendHtml(this.RenderMenuItem(
                "nav-link",
                ForumPages.Profile_EditAvatar,
                this.GetText("ACCOUNT", "EDIT_AVATAR"),
                "user-tie"));
        }

        if (this.PageContext.BoardSettings.AllowSignatures)
        {
            html.InnerHtml.AppendHtml(this.RenderMenuItem(
                "nav-link",
                ForumPages.Profile_EditSignature,
                this.GetText("ACCOUNT", "SIGNATURE"),
                "signature"));
        }

        html.InnerHtml.AppendHtml(this.RenderMenuItem(
            "nav-link",
            ForumPages.Profile_Subscriptions,
            this.GetText("ACCOUNT", "SUBSCRIPTIONS"),
            "envelope"));

        html.InnerHtml.AppendHtml(this.RenderMenuItem(
            "nav-link",
            ForumPages.Profile_BlockOptions,
            this.GetText("BLOCK_OPTIONS", "TITLE"),
            "user-lock"));

        // Render Change Password Item
        html.InnerHtml.AppendHtml(this.RenderMenuItem(
            "nav-link",
            ForumPages.Profile_ChangePassword,
            this.GetText("ACCOUNT", "CHANGE_PASSWORD"),
            "lock"));

        if (!this.PageContext.PageUser.UserFlags.IsHostAdmin)
        {
            // Render Delete Account Item
            html.InnerHtml.AppendHtml(this.RenderMenuItem(
                "nav-link text-danger",
                ForumPages.Profile_DeleteAccount,
                this.GetText("ACCOUNT", "DELETE_ACCOUNT"),
                "user-alt-slash"));
        }

        output.Content.AppendHtml(html);
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
    private TagBuilder RenderMenuItem(
        string cssClass,
        ForumPages page,
        string getText,
        string iconName,
        object parameter = null)
    {
        var link = new TagBuilder(HtmlTag.A);

        if (BoardContext.Current.CurrentForumPage.PageName == page ||
            (BoardContext.Current.CurrentForumPage.PageName == ForumPages.Notification && page == ForumPages.MyAccount))
        {
            link.AddCssClass($"{cssClass} active");
        }
        else
        {
            link.AddCssClass(cssClass);
        }

        link.MergeAttribute(
            HtmlAttribute.Href,
            parameter is not null
                ? this.Get<ILinkBuilder>().GetLink(page, parameter)
                : this.Get<ILinkBuilder>().GetLink(page));

        link.MergeAttribute("data-bs-toggle", "tooltip");
        link.MergeAttribute(HtmlAttribute.Title, getText);

        var icon = new TagBuilder(HtmlTag.I);

        if (page == ForumPages.Profile_DeleteAccount)
        {
            icon.AddCssClass(
                $"fas fa-{iconName} me-1 text-danger");
        }
        else
        {
            icon.AddCssClass(
                $"fas fa-{iconName} me-1 {(BoardContext.Current.CurrentForumPage.PageName == page ? "text-light" : "text-secondary")}");
        }

        link.InnerHtml.AppendHtml(icon);

        var span = new TagBuilder(HtmlTag.Span);

        span.AddCssClass("d-none d-lg-inline-block");

        span.InnerHtml.Append(getText);

        link.InnerHtml.AppendHtml(span);

        return link;
    }
}