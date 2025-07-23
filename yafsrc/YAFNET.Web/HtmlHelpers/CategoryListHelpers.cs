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
/// The Category List html helper.
/// </summary>
public static class CategoryListHelpers
{
    /// <summary>
    /// Gets the forum icon.
    /// </summary>
    /// <param name="htmlHelper"></param>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <param name="lastRead">
    /// The last Read.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent GetForumIcon(this IHtmlHelper htmlHelper, ForumRead item, DateTime lastRead)
    {
        var content = new HtmlContentBuilder();

        var lastPosted = item.LastPosted ?? lastRead;

        if (item.ImageURL.IsNotSet())
        {
            var forumIconNew = htmlHelper.Icon("comments", "text-success", "fas", "fa-2x");

            var forumIconNormal = htmlHelper
                .Icon("comments", "text-secondary", "fas", "fa-2x");

            var forumIconLocked = htmlHelper.IconStack(
                "comments",
                "text-secondary",
                "lock",
                "text-warning",
                "fa-1x");

            var iconLink = new TagBuilder(HtmlTag.A);

            iconLink.MergeAttribute(HtmlAttribute.Tabindex, "0");
            iconLink.MergeAttribute(HtmlAttribute.Role, HtmlTag.Button);
            iconLink.MergeAttribute("data-bs-toggle", "popover");
            iconLink.MergeAttribute(HtmlAttribute.Href, "#");
            iconLink.MergeAttribute(HtmlAttribute.AriaLabel, "icon-legend");

            iconLink.AddCssClass("btn btn-link m-0 p-0 forum-icon-legend-popvover");

            if (item.ForumFlags.IsLocked)
            {
                iconLink.InnerHtml.AppendHtml(forumIconLocked);
            }
            else if (lastPosted > lastRead && item.ReadAccess)
            {
                iconLink.InnerHtml.AppendHtml(forumIconNew);
            }
            else
            {
                iconLink.InnerHtml.AppendHtml(forumIconNormal);
            }

            content.AppendHtml(iconLink);
        }
        else
        {
            var forumImage = new TagBuilder(HtmlTag.Img);

            forumImage.MergeAttribute("src", $"/{BoardContext.Current.Get<BoardFolders>().Forums}/{item.ImageURL}");
            forumImage.MergeAttribute("data-bs-toggle", "tooltip");

            // Highlight custom icon images and add tool tips to them.
            if (item.ForumFlags.IsLocked)
            {
                forumImage.AddCssClass("forum_customimage_locked");
                forumImage.MergeAttribute(
                    "alt",
                    BoardContext.Current.Get<ILocalization>().GetText("ICONLEGEND", "FORUM_LOCKED"));
                forumImage.MergeAttribute(
                    HtmlAttribute.Title,
                    BoardContext.Current.Get<ILocalization>().GetText("ICONLEGEND", "FORUM_LOCKED"));
            }
            else if (lastPosted > lastRead)
            {
                forumImage.AddCssClass("forum_customimage_newposts");
                forumImage.MergeAttribute(
                    "alt",
                    BoardContext.Current.Get<ILocalization>().GetText("ICONLEGEND", "NEW_POSTS"));
                forumImage.MergeAttribute(
                    HtmlAttribute.Title,
                    BoardContext.Current.Get<ILocalization>().GetText("ICONLEGEND", "NEW_POSTS"));
            }
            else
            {
                forumImage.AddCssClass("forum_customimage_nonewposts");
                forumImage.MergeAttribute(
                    "alt",
                    BoardContext.Current.Get<ILocalization>().GetText("ICONLEGEND", "NO_NEW_POSTS"));
                forumImage.MergeAttribute(
                    HtmlAttribute.Title,
                    BoardContext.Current.Get<ILocalization>().GetText("ICONLEGEND", "NO_NEW_POSTS"));
            }

            content.AppendHtml(forumImage);
        }

        return content;
    }

    /// <summary>
    /// Provides the "Forum Link Text" for the ForumList control.
    ///   Automatically disables the link if the current user doesn't
    ///   have proper permissions.
    /// </summary>
    /// <param name="htmlHelper"></param>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// Forum link text
    /// </returns>
    public static IHtmlContent GetForumLink(this IHtmlHelper htmlHelper, ForumRead item)
    {
        var content = new HtmlContentBuilder();

        if (item.ReadAccess)
        {
            var title = item.Description.IsSet()
                            ? item.Description
                            : BoardContext.Current.Get<ILocalization>().GetText("COMMON", "VIEW_FORUM");

            var link = new TagBuilder(HtmlTag.A);

            link.MergeAttribute("data-bs-toggle", "tooltip");

            link.InnerHtml.AppendHtml(htmlHelper.HtmlEncode(item.Forum));

            if (item.RemoteURL.IsSet())
            {
                link.MergeAttribute("href", item.RemoteURL);
                link.MergeAttribute(HtmlAttribute.Title, BoardContext.Current.Get<ILocalization>().GetText("COMMON", "VIEW_FORUM"));
                link.MergeAttribute(HtmlAttribute.Target, "_blank");

                var icon = new TagBuilder(HtmlTag.I);

                icon.AddCssClass("fas fa-external-link-alt ms-1");

                link.InnerHtml.AppendHtml(icon);
            }
            else
            {
                link.MergeAttribute(
                    HtmlAttribute.Href,
                    BoardContext.Current.Get<ILinkBuilder>().GetForumLink(item.ForumID, item.Forum));
                link.MergeAttribute(HtmlAttribute.Title, title);
            }

            link.MergeAttribute(
                HtmlAttribute.Href,
                item.RemoteURL.IsSet()
                    ? item.RemoteURL
                    : BoardContext.Current.Get<ILinkBuilder>().GetForumLink(item.ForumID, item.Forum));

            content.AppendHtml(link);
        }
        else
        {
            // no access to this forum
            content.AppendHtml($"{item.Forum} {BoardContext.Current.Get<ILocalization>().GetText("NO_FORUM_ACCESS")}");
        }

        return content;
    }

    /// <summary>
    /// Get moderators.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public static string GetModerators(this IHtmlHelper htmlHelper, ForumRead item)
    {
        var mods = BoardContext.Current.Get<ISessionService>().Mods.Where(x => x.ForumID == item.ForumID).ToList();

        var content = new HtmlContentBuilder();

        var list = new TagBuilder(HtmlTag.Ol);

        list.AddCssClass("list-unstyled");

        mods.DistinctBy(x => x.ModeratorID).ForEach(
            row =>
                {
                    var listItem = new TagBuilder(HtmlTag.Li);

                    if (row.IsGroup)
                    {
                        // render mod group
                        listItem.InnerHtml.Append(
                            BoardContext.Current.BoardSettings.EnableDisplayName ? row.DisplayName : row.Name);
                    }
                    else
                    {
                        // Render Moderator PageUser Link
                        var userLink = htmlHelper.UserLink(
                            row.ModeratorID,
                            BoardContext.Current.BoardSettings.EnableDisplayName ? row.DisplayName : row.Name,
                            null,
                            row.Style);

                        listItem.InnerHtml.AppendHtml(userLink);
                    }

                    list.InnerHtml.AppendHtml(listItem);
                });

        content.AppendHtml(list);

        return content.RenderToString().ToJsString();
    }

    /// <summary>
    /// Provides the "Forum Link Text" for the ForumList control.
    ///   Automatically disables the link if the current user doesn't
    ///   have proper permissions.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// Forum link text
    /// </returns>
    public static IHtmlContent GetSubForumLink(this IHtmlHelper htmlHelper, ForumRead item)
    {
        var content = new HtmlContentBuilder();

        if (item.ReadAccess)
        {
            var title = item.Description.IsSet()
                            ? item.Description
                            : BoardContext.Current.Get<ILocalization>().GetText("COMMON", "VIEW_FORUM");

            var link = new TagBuilder(HtmlTag.A);

            link.AddCssClass("card-link small");

            link.MergeAttribute("data-bs-toggle", "tooltip");

            link.InnerHtml.AppendHtml(BoardContext.Current.CurrentForumPage.HtmlEncode(item.Forum));

            link.MergeAttribute(HtmlAttribute.Href, BoardContext.Current.Get<ILinkBuilder>().GetForumLink(item.ForumID, item.Forum));
            link.MergeAttribute(HtmlAttribute.Title, title);

            link.MergeAttribute(
                HtmlAttribute.Href,
                item.RemoteURL.IsSet()
                    ? item.RemoteURL
                    : BoardContext.Current.Get<ILinkBuilder>().GetForumLink(item.ForumID, item.Forum));

            content.AppendHtml(link);
        }
        else
        {
            // no access to this forum
            content.AppendHtml($"{item.Forum} {BoardContext.Current.Get<ILocalization>().GetText("NO_FORUM_ACCESS")}");
        }

        return content;
    }

    /// <summary>
    /// Gets the last post info.
    /// </summary>
    /// <param name="htmlHelper"></param>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public static string GetLastPostInfo(this IHtmlHelper htmlHelper, ForumRead item)
    {
        // Last Post Date
        var lastPostedDateTime = item.LastPosted.Value;

        // Last Topic PageUser
        var lastUserLink = htmlHelper.UserLink(
            item.LastUserID.Value,
            BoardContext.Current.BoardSettings.EnableDisplayName ? item.LastUserDisplayName : item.LastUser,
            item.LastUserSuspended,
            BoardContext.Current.BoardSettings.UseStyledNicks && item.Style.IsSet()
                ? BoardContext.Current.Get<IStyleTransform>().Decode(item.Style)
                : string.Empty,
            true);

        var formattedDatetime = BoardContext.Current.BoardSettings.ShowRelativeTime
                                    ? lastPostedDateTime.ToRelativeTime()
                                    : BoardContext.Current.Get<IDateTimeService>().Format(
                                        DateTimeFormat.BothTopic,
                                        lastPostedDateTime);

        var span = BoardContext.Current.BoardSettings.ShowRelativeTime ? @"<span class=""popover-timeago"">" : "<span>";

        var iconBadge = htmlHelper.IconBadge("calendar-day", "clock");

        return $"""
                                          {lastUserLink.RenderToString()}
                                          {iconBadge.RenderToString()}&nbsp;{span}{formattedDatetime}</span>
                                         
                """;
    }

    /// <summary>
    /// Gets the sub Forums.
    /// </summary>
    /// <param name="htmlHelper"></param>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// Returns the Sub Forums
    /// </returns>
    public static IEnumerable<ForumRead> GetSubForums(this IHtmlHelper htmlHelper, ForumRead item)
    {
        return BoardContext.Current.Get<ISessionService>().Forums.Where(forum => forum.ParentID == item.ForumID)
            .Take(BoardContext.Current.BoardSettings.SubForumsInForumList);
    }
}