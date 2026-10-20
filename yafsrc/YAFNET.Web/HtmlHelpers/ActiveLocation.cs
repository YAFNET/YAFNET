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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Web.HtmlHelpers;

using System.Net;

/// <summary>
/// The active location html helper.
/// </summary>
public static class ActiveLocationHtmlHelper
{
    /// <summary>
    /// The texts of the pages whose name is not the tag of the text (the missing text was shown as
    /// [ACTIVELOCATION.PAGE] and logged).
    /// </summary>
    private readonly static Dictionary<ForumPages, string> PageTags = new()
    {
        { ForumPages.Post, "POSTS" },
        { ForumPages.PostTopic, "POSTTOPIC" },
        { ForumPages.PostMessage, "POSTMESSAGE_FULL" },
        { ForumPages.EditMessage, "POSTS" },
        { ForumPages.PollEdit, "POSTS" },
        { ForumPages.MyMessages, "PM" },
        { ForumPages.MyFriends, "FRIENDS" },
        { ForumPages.Notification, "NOTIFICATION" },
        { ForumPages.EditAlbumImages, "EDIT_ALBUMIMAGES" },
        { ForumPages.Privacy, "RULES" },
        { ForumPages.Moderate_Moderate, "MODERATE" },
        { ForumPages.Moderate_Forums, "MODERATE" },
        { ForumPages.Email, "IM_EMAIL" },
        { ForumPages.Help, "HELP_INDEX" },
        { ForumPages.Digest, "ADMINTASK" },
        { ForumPages.SiteMap, "NODATA" },
        { ForumPages.Account_Approve, "APPROVE" },
        { ForumPages.Account_ForgotPassword, "RECOVERPASSWORD" },
        { ForumPages.Account_ResetPassword, "RECOVERPASSWORD" },
        { ForumPages.Account_Logout, "LOGOUT" },
        { ForumPages.Account_Register, "REGISTER" },
        { ForumPages.Account_Login, "LOGIN" },
        { ForumPages.Account_Authorize, "LOGIN" },
        { ForumPages.MyAccount, "ACCOUNT" },
        { ForumPages.Admin_HostSettings, "ADMIN_HOSTSETTING" },
        { ForumPages.Admin_Settings, "ADMIN_BOARDSETTINGS" },
        { ForumPages.Admin_BannedCountries, "ADMIN_BANNED_COUNTRIES" },
        { ForumPages.Admin_BannedEmails, "ADMIN_BANNEDEMAIL" },
        { ForumPages.Admin_BannedIps, "ADMIN_BANNEDIP" },
        { ForumPages.Admin_BannedNames, "ADMIN_BANNEDNAME" },
        { ForumPages.Admin_BannedUserAgents, "ADMIN_BANNED_USERAGENTS" },
        { ForumPages.Admin_RegisterUser, "ADMIN_REGUSER" },
        { ForumPages.Admin_BBCodes, "ADMIN_BBCODE" },
        { ForumPages.Admin_EditBBCode, "ADMIN_BBCODE_EDIT" },
        { ForumPages.Admin_BoardAnnouncement, "ADMINTASK" },
        { ForumPages.Admin_PageAccessList, "ADMINTASK" },
        { ForumPages.Admin_PageAccessEdit, "ADMINTASK" },
        { ForumPages.Admin_Restore, "ADMINTASK" },
        { ForumPages.Admin_TestData, "ADMINTASK" },
        { ForumPages.Admin_Digest, "ADMINTASK" },
        { ForumPages.Admin_Attachments, "ADMINTASK" },
        { ForumPages.Admin_Tags, "ADMINTASK" },
        { ForumPages.Profile_Attachments, "ATTACHMENTS" },
        { ForumPages.Profile_BlockOptions, "BLOCK_OPTIONS" },
        { ForumPages.Profile_ChangePassword, "CHANGE_PASSWORD" },
        { ForumPages.Profile_DeleteAccount, "DELETE_ACCOUNT" },
        { ForumPages.Profile_EditAvatar, "EDIT_AVATAR" },
        { ForumPages.Profile_EditProfile, "EDIT_PROFILE" },
        { ForumPages.Profile_EditSettings, "EDIT_SETTINGS" },
        { ForumPages.Profile_EditSignature, "EDIT_SIGNATURE" },
        { ForumPages.Profile_Subscriptions, "SUBSCRIPTIONS" }
    };

    /// <summary>
    /// Gets the tag of the text (of the page ACTIVELOCATION) of a page.
    /// </summary>
    /// <param name="forumPage">The page.</param>
    /// <returns>Returns the tag.</returns>
    public static string LocationTag(string forumPage)
    {
        return Enum.TryParse<ForumPages>(forumPage, true, out var page) && PageTags.TryGetValue(page, out var tag)
                   ? tag
                   : forumPage.ToUpper();
    }

    /// <summary>
    /// Provides Active Users location info
    /// </summary>
    /// <param name="_">
    /// The html helper.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="hasForumAccess">
    /// The has forum access.
    /// </param>
    /// <param name="forumPage">
    /// The forum page.
    /// </param>
    /// <param name="path">
    /// The url path.
    /// </param>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <param name="forumName">
    /// The forum name.
    /// </param>
    /// <param name="topicId">
    /// The topic id.
    /// </param>
    /// <param name="topicName">
    /// The topic name.
    /// </param>
    /// <param name="lastLinkOnly">
    /// The last link only.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent ActiveLocation(
        this IHtmlHelper _,
        int userId,
        bool hasForumAccess,
        string forumPage,
        string path,
        int forumId,
        string forumName,
        int topicId,
        string topicName,
        bool lastLinkOnly)
    {
        var content = new HtmlContentBuilder();

        var context = BoardContext.Current;

        if (forumPage.IsNotSet())
        {
            forumPage = "Index";
        }

        var forumPageName = forumPage.ToEnum<ForumPages>();

        switch (forumPageName)
        {
            case ForumPages.Index:
                content.Append(
                    context.Get<ILocalization>().GetText("ACTIVELOCATION", "MAINPAGE"));
                break;
            case ForumPages.Albums:
                content.AppendHtml(RenderAlbumsLocation(path, userId));
                break;
            case ForumPages.Album:
                content.AppendHtml(RenderAlbumLocation(path, userId));
                break;
            case ForumPages.UserProfile:
                content.AppendHtml(RenderProfileLocation(path, userId));
                break;
            case ForumPages.Topics:
                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "FORUM"));

                if (!hasForumAccess)
                {
                    return content;
                }

                var lastLink = new TagBuilder(HtmlTag.A);

                lastLink.MergeAttribute("data-bs-toggle", "tooltip");
                lastLink.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("COMMON", "VIEW_FORUM"));
                lastLink.MergeAttribute(HtmlAttribute.Href, context.Get<ILinkBuilder>().GetForumLink(forumId, forumName));

                lastLink.InnerHtml.Append(forumName);

                content.AppendHtml(lastLink);

                break;
            case ForumPages.PostTopic:
                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "POSTTOPIC"));

                if (!hasForumAccess || forumId <= 0)
                {
                    return content;
                }

                var forumLink = new TagBuilder(HtmlTag.A);

                forumLink.MergeAttribute("data-bs-toggle", "tooltip");
                forumLink.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("COMMON", "VIEW_FORUM"));
                forumLink.MergeAttribute(HtmlAttribute.Href, context.Get<ILinkBuilder>().GetForumLink(forumId, forumName));

                forumLink.InnerHtml.Append(forumName);

                content.AppendHtml(forumLink);

                break;
            default:
                if (!BoardContext.Current.IsAdmin && forumPage.Contains("MODERATE_", StringComparison.CurrentCultureIgnoreCase))
                {
                    // We shouldn't show moderators activity to all users but admins
                    content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "MODERATE"));
                }
                else if (!BoardContext.Current.PageUser.UserFlags.IsHostAdmin &&
                         forumPage.Contains("ADMIN_", StringComparison.CurrentCultureIgnoreCase) || forumPage.Contains("HOST_", StringComparison.CurrentCultureIgnoreCase))
                {
                    // We shouldn't show admin activity to all users
                    content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "ADMINTASK"));
                }
                else
                {
                    if (topicId > 0 && forumId > 0)
                    {
                        content.AppendHtml(RenderTopicsOrForumLocations(forumPageName, hasForumAccess, lastLinkOnly, topicId, topicName, forumId, forumName));
                    }
                    else
                    {
                        // Generic action name based on page name
                        content.Append(
                            context.Get<ILocalization>().GetText("ACTIVELOCATION", LocationTag(forumPage)));
                    }
                }

                break;
        }

        return content;
    }

    /// <summary>
    /// The render topics or forum locations.
    /// </summary>
    /// <param name="forumPageName">
    /// The forum page name.
    /// </param>
    /// <param name="hasForumAccess">
    /// The has Forum Access.
    /// </param>
    /// <param name="lastLinkOnly">
    /// The last Link Only.
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    /// <param name="topicName">
    /// The topic Name.
    /// </param>
    /// <param name="forumId">
    /// The forum Id.
    /// </param>
    /// <param name="forumName">
    /// The forum Name.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    private static HtmlContentBuilder RenderTopicsOrForumLocations(
        ForumPages forumPageName,
        bool hasForumAccess,
        bool lastLinkOnly,
        int topicId,
        string topicName,
        int forumId,
        string forumName)
    {
        var context = BoardContext.Current;

        var content = new HtmlContentBuilder();

        // All pages should be processed in call frequency order
        // We are in messages
        switch (forumPageName)
        {
            case ForumPages.Post:
            case ForumPages.Posts:
                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "POSTS"));
                break;

            case ForumPages.PostMessage:
                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "POSTMESSAGE_FULL"));
                break;

            case ForumPages.ReportPost:
                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "REPORTPOST"));
                content.Append(". ");
                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "TOPICS"));
                break;

            case ForumPages.MessageHistory:
                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "MESSAGEHISTORY"));
                content.Append(". ");
                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "TOPICS"));
                break;

            default:
                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "POSTS"));
                break;
        }

        if (!hasForumAccess)
        {
            return content;
        }

        var link = new TagBuilder(HtmlTag.A);

        link.MergeAttribute("data-bs-toggle", "tooltip");
        link.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("COMMON", "VIEW_TOPIC"));
        link.MergeAttribute(HtmlAttribute.Href, context.Get<ILinkBuilder>().GetTopicLink(topicId, topicName));

        link.InnerHtml.Append(topicName);

        content.AppendHtml(link);

        if (lastLinkOnly)
        {
            return content;
        }

        content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "TOPICINFORUM"));

        var lastLink = new TagBuilder(HtmlTag.A);

        lastLink.MergeAttribute("data-bs-toggle", "tooltip");
        lastLink.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("COMMON", "VIEW_FORUM"));
        lastLink.MergeAttribute(HtmlAttribute.Href, context.Get<ILinkBuilder>().GetForumLink(forumId, forumName));

        lastLink.InnerHtml.Append(forumName);

        content.AppendHtml(lastLink);

        return content;
    }

    /// <summary>
    /// A method to get album path string.
    /// </summary>
    /// <param name="path">
    /// The url path.
    /// </param>
    /// <param name="currentUserId">
    /// The current PageUser Id.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    private static HtmlContentBuilder RenderAlbumLocation(
        string path,
        int currentUserId)
    {
        var context = BoardContext.Current;

        var content = new HtmlContentBuilder();

        if (path.Contains("api"))
        {
            content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "ALBUM"));
            return content;
        }

        // the path is /Album/{user id}/{album id}[/{page}]
        var parts = path.Replace("/Album/", string.Empty).Split('/', StringSplitOptions.RemoveEmptyEntries);

        var album = parts.Length > 1 && int.TryParse(parts[1], out var albumId)
                        ? context.GetRepository<UserAlbum>().GetById(albumId)
                        : null;

        // the album can be deleted or the id can be invalid (the path is the address the user called)
        var owner = album is null ? null : context.GetRepository<User>().GetById(album.UserID);

        if (owner is null)
        {
            content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "ALBUM"));
            return content;
        }

        // If album doesn't have a Title, use his ID (the title is stored html encoded).
        var albumName = album.Title.IsSet() ? WebUtility.HtmlDecode(album.Title) : album.ID.ToString();

        var link = new TagBuilder(HtmlTag.A);

        link.MergeAttribute(
            HtmlAttribute.Href,
            context.Get<ILinkBuilder>().GetLink(ForumPages.Album, new { u = owner.ID, a = album.ID }));

        link.InnerHtml.Append(albumName);

        if (owner.ID != currentUserId)
        {
            content.Append($"{context.Get<ILocalization>().GetText("ACTIVELOCATION", "ALBUM")} ");

            content.AppendHtml(link);

            content.Append($" {context.Get<ILocalization>().GetText("ACTIVELOCATION", "ALBUM_OFUSER")} ");

            var linkProfile = new TagBuilder(HtmlTag.A);

            linkProfile.MergeAttribute(
                HtmlAttribute.Href,
                context.Get<ILinkBuilder>().GetUserProfileLink(owner.ID, owner.DisplayOrUserName()));
            linkProfile.MergeAttribute("data-bs-toggle", "tooltip");
            linkProfile.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("COMMON", "VIEW_USRPROFILE"));

            linkProfile.InnerHtml.Append(owner.DisplayOrUserName());

            content.AppendHtml(linkProfile);
        }
        else
        {
            content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "ALBUM_OWN"));

            content.AppendHtml(link);
        }

        return content;
    }

    /// <summary>
    /// A method to get albums path string.
    /// </summary>
    /// <param name="path">
    /// The url path.
    /// </param>
    /// <param name="currentUserId">
    /// The current PageUser Id.
    /// </param>
    /// <returns>
    /// The string
    /// </returns>
    private static HtmlContentBuilder RenderAlbumsLocation(
        string path,
        int currentUserId)
    {
        var context = BoardContext.Current;

        var content = new HtmlContentBuilder();

        if (path.Contains("api"))
        {
            content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "ALBUMS"));
            return content;
        }

        // the path is /Albums/{user id}[/{page}] (the name is not in the path)
        var user = GetPathUser(path.Replace("/Albums/", string.Empty));

        if (user is null)
        {
            content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "ALBUMS"));
            return content;
        }

        var userId = user.ID;
        var userName = user.DisplayOrUserName();

        if (userId != currentUserId)
        {
            content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "ALBUMS_OFUSER"));

            content.AppendHtml("&nbsp;");

            var link = new TagBuilder(HtmlTag.A);

            link.MergeAttribute(
                HtmlAttribute.Href,
                context.Get<ILinkBuilder>().GetUserProfileLink(userId, userName));
            link.MergeAttribute("data-bs-toggle", "tooltip");
            link.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("COMMON", "VIEW_USRPROFILE"));

            link.InnerHtml.Append(userName);

            content.AppendHtml(link);
        }
        else
        {
            content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "ALBUMS_OWN"));
        }

        return content;
    }

    /// <summary>
    /// A method to get profile path string.
    /// </summary>
    /// <param name="path">
    /// The url path.
    /// </param>
    /// <param name="currentUserId">
    /// The current PageUser Id.
    /// </param>
    /// <returns>
    /// The profile.
    /// </returns>
    private static HtmlContentBuilder RenderProfileLocation(
        string path,
        int currentUserId)
    {
        var context = BoardContext.Current;

        var content = new HtmlContentBuilder();

        // the path is /UserProfile/{user id}/{name}, the name is any text of the address (not shown)
        var user = GetPathUser(path.Replace("/UserProfile/", string.Empty));

        if (user is null)
        {
            content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "PROFILE_OFUSER"));
            return content;
        }

        var userId = user.ID;
        var userName = user.DisplayOrUserName();

        if (userId != currentUserId)
        {
            content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "PROFILE_OFUSER"));

            content.AppendHtml("&nbsp;");

            var link = new TagBuilder(HtmlTag.A);

            link.MergeAttribute(
                HtmlAttribute.Href,
                context.Get<ILinkBuilder>().GetUserProfileLink(userId, userName));
            link.MergeAttribute("data-bs-toggle", "tooltip");
            link.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("COMMON", "VIEW_USRPROFILE"));

            link.InnerHtml.Append(userName);

            content.AppendHtml(link);
        }
        else
        {
            content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "PROFILE_OWN"));
        }

        return content;
    }

    /// <summary>
    /// Gets the user of the first part of the path (the user id), null if it is no user of the board.
    /// </summary>
    /// <param name="path">The path without the page name.</param>
    /// <returns>The user or null.</returns>
    private static User GetPathUser(string path)
    {
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0 || !int.TryParse(parts[0], out var userId))
        {
            return null;
        }

        var user = BoardContext.Current.GetRepository<User>().GetById(userId);

        return user is not null && user.BoardID == BoardContext.Current.PageBoardID ? user : null;
    }
}
