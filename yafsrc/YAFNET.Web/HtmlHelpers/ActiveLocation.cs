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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Web.HtmlHelpers;

/// <summary>
/// The active location html helper.
/// </summary>
public static class ActiveLocationHtmlHelper
{
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
    /// <param name="location">
    /// The location.
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
        string location,
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
                    location.Contains("c=")
                        ? context.Get<ILocalization>().GetText("ACTIVELOCATION", "FORUMFROMCATEGORY")
                        : context.Get<ILocalization>().GetText("ACTIVELOCATION", "MAINPAGE"));
                break;
            case ForumPages.Albums:
                content.AppendHtml(RenderAlbumsLocation(location, userId));
                break;
            case ForumPages.Album:
                content.AppendHtml(RenderAlbumLocation(location, userId));
                break;
            case ForumPages.UserProfile:
                content.AppendHtml(RenderProfileLocation(location, userId));
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
                lastLink.MergeAttribute(HtmlAttribute.Href, context.Get<LinkBuilder>().GetForumLink(forumId, forumName));

                lastLink.InnerHtml.Append(forumName);

                content.AppendHtml(lastLink);

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
                            context.Get<ILocalization>().GetText("ACTIVELOCATION", forumPage.ToUpper()));
                    }
                }

                break;
        }

        return content;
    }

    /// <summary>
    /// Gets the PageUser id from query string.
    /// </summary>
    /// <param name="queryString">
    /// The query string.
    /// </param>
    /// <returns>
    /// Returns the PageUser Id
    /// </returns>
    private static int? GetUserIdFromQueryString(string queryString)
    {
        if (ValidationHelper.IsValidInt(queryString))
        {
            return queryString.ToType<int>();
        }

        return null;
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
        link.MergeAttribute(HtmlAttribute.Href, context.Get<LinkBuilder>().GetTopicLink(topicId, topicName));

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
        lastLink.MergeAttribute(HtmlAttribute.Href, context.Get<LinkBuilder>().GetForumLink(forumId, forumName));

        lastLink.InnerHtml.Append(forumName);

        content.AppendHtml(lastLink);

        return content;
    }

    /// <summary>
    /// A method to get album path string.
    /// </summary>
    /// <param name="forumPageAttributes">
    /// A page query string cleared from page name.
    /// </param>
    /// <param name="currentUserId">
    /// The current PageUser Id.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    private static HtmlContentBuilder RenderAlbumLocation(
        string forumPageAttributes,
        int currentUserId)
    {
        var context = BoardContext.Current;

        var content = new HtmlContentBuilder();

        var userId = GetUserIdFromQueryString(forumPageAttributes);

        var albumId =
            forumPageAttributes[(forumPageAttributes.IndexOf("a=", StringComparison.Ordinal) + 2)..];

        albumId = albumId.Contains('&')
                      ? albumId[..albumId.IndexOf('&')].Trim()
                      : albumId[..].Trim();

        if (userId.HasValue && ValidationHelper.IsValidInt(albumId))
        {
            // The DataRow should not be missing in the case
            var userAlbum = context.GetRepository<UserAlbum>().GetById(albumId.Trim().ToType<int>());

            // If album doesn't have a Title, use his ID.
            var albumName = userAlbum.Title.IsNotSet() ? userAlbum.Title : userAlbum.ID.ToString();

            // Render
            if (userId != currentUserId)
            {
                var user = context.GetRepository<User>().GetById(userId.Value);

                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "ALBUM"));

                var link = new TagBuilder(HtmlTag.A);

                link.MergeAttribute(
                    HtmlAttribute.Href,
                    context.Get<LinkBuilder>().GetLink(ForumPages.Album, new { a = albumId }));

                link.InnerHtml.Append(albumName);

                content.AppendHtml(link);

                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "ALBUM_OFUSER"));

                var linkProfile = new TagBuilder(HtmlTag.A);

                linkProfile.MergeAttribute(
                    HtmlAttribute.Href,
                    context.Get<LinkBuilder>().GetUserProfileLink(userId.Value, user.DisplayOrUserName()));
                link.MergeAttribute("data-bs-toggle", "tooltip");
                link.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("COMMON", "VIEW_USRPROFILE"));

                linkProfile.InnerHtml.Append(user.DisplayOrUserName());

                content.AppendHtml(linkProfile);
            }
            else
            {
                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "ALBUM_OWN"));

                var link = new TagBuilder(HtmlTag.A);

                link.MergeAttribute(
                    HtmlAttribute.Href,
                    context.Get<LinkBuilder>().GetLink(ForumPages.Album, new { a = albumId }));

                link.InnerHtml.Append(albumName);

                content.AppendHtml(link);
            }
        }
        else
        {
            content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "ALBUM"));
        }

        return content;
    }

    /// <summary>
    /// A method to get albums path string.
    /// </summary>
    /// <param name="forumPageAttributes">
    /// A page query string cleared from page name.
    /// </param>
    /// <param name="currentUserId">
    /// The current PageUser Id.
    /// </param>
    /// <returns>
    /// The string
    /// </returns>
    private static HtmlContentBuilder RenderAlbumsLocation(
        string forumPageAttributes,
        int currentUserId)
    {
        var context = BoardContext.Current;

        var content = new HtmlContentBuilder();

        var userId = GetUserIdFromQueryString(forumPageAttributes);

        if (userId.HasValue)
        {
            if (userId.Value != currentUserId)
            {
                var user = context.GetRepository<User>().GetById(userId.Value);

                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "ALBUMS_OFUSER"));

                content.AppendHtml("&nbsp;");

                var link = new TagBuilder(HtmlTag.A);

                link.MergeAttribute(
                    HtmlAttribute.Href,
                    context.Get<LinkBuilder>().GetUserProfileLink(userId.Value, user.DisplayOrUserName()));
                link.MergeAttribute("data-bs-toggle", "tooltip");
                link.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("COMMON", "VIEW_USRPROFILE"));

                link.InnerHtml.Append(user.DisplayOrUserName());

                content.AppendHtml(link);
            }
            else
            {
                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "ALBUMS_OWN"));
            }
        }
        else
        {
            content.Append(context.Get<ILocalization>().GetTextFormatted("ACTIVELOCATION", "ALBUMS"));
        }

        return content;
    }

    /// <summary>
    /// A method to get profile path string.
    /// </summary>
    /// <param name="forumPageAttributes">
    /// The forum page attributes.
    /// </param>
    /// <param name="currentUserId">
    /// The current PageUser Id.
    /// </param>
    /// <returns>
    /// The profile.
    /// </returns>
    private static HtmlContentBuilder RenderProfileLocation(
        string forumPageAttributes,
        int currentUserId)
    {
        var context = BoardContext.Current;

        var content = new HtmlContentBuilder();

        var userId = GetUserIdFromQueryString(forumPageAttributes);

        if (userId.HasValue)
        {
            if (userId.Value != currentUserId)
            {
                var user = context.GetRepository<User>().GetById(userId.Value);

                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "PROFILE_OFUSER"));

                content.AppendHtml("&nbsp;");

                var link = new TagBuilder(HtmlTag.A);

                link.MergeAttribute(
                    HtmlAttribute.Href,
                    context.Get<LinkBuilder>().GetUserProfileLink(userId.Value, user.DisplayOrUserName()));
                link.MergeAttribute("data-bs-toggle", "tooltip");
                link.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("COMMON", "VIEW_USRPROFILE"));

                link.InnerHtml.Append(user.DisplayOrUserName());

                content.AppendHtml(link);
            }
            else
            {
                content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "PROFILE_OWN"));
            }
        }
        else
        {
            content.Append(context.Get<ILocalization>().GetText("ACTIVELOCATION", "PROFILE"));
        }

        return content;
    }
}