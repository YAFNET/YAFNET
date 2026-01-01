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

/// <summary>
/// The active user stats html helper.
/// </summary>
public static class ActiveUserStatsHtmlHelper
{
    /// <summary>
    /// The active user stats.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="activeUsers">
    /// The active users.
    /// </param>
    /// <param name="activeHidden">
    /// The active hidden.
    /// </param>
    /// <param name="activeMembers">
    /// The active members.
    /// </param>
    /// <param name="activeGuests">
    /// The active guests.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent ActiveUserStats(
        this IHtmlHelper htmlHelper,
        int activeUsers,
        int activeHidden,
        int activeMembers,
        int activeGuests)
    {
        var content = new HtmlContentBuilder();

        var context = BoardContext.Current;

        // show hidden count to admin...
        if (context.IsAdmin)
        {
            activeUsers += activeHidden;
        }

        var canViewActive = context.Get<IPermissions>().Check(context.BoardSettings.ActiveUsersViewPermissions);
        var showGuestTotal = activeGuests > 0 && (context.BoardSettings.ShowGuestsInDetailedActiveList ||
                                                  context.BoardSettings.ShowCrawlersInActiveList);

        if (canViewActive && (showGuestTotal || activeMembers > 0 && activeGuests <= 0))
        {
            // always show active users...
            var link = new TagBuilder(HtmlTag.A);

            link.MergeAttribute(HtmlAttribute.Href, context.Get<ILinkBuilder>().GetLink(ForumPages.ActiveUsers, new { v = 0 }));
            link.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("COMMON", "VIEW_FULLINFO"));
            link.MergeAttribute("data-bs-toggle", "tooltip");

            if (context.IsCrawler)
            {
                link.MergeAttribute(HtmlAttribute.Rel, "nofollow");
            }

            link.InnerHtml.Append(
                context.Get<ILocalization>().GetTextFormatted(
                    activeUsers == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2",
                    activeUsers));

            content.AppendHtml(link);
        }
        else
        {
            // no link because no permissions...
            content.Append(
                context.Get<ILocalization>().GetTextFormatted(
                    activeUsers == 1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2",
                    activeUsers));
        }

        if (activeMembers > 0)
        {
            content.Append(", ");

            if (canViewActive)
            {
                var link = new TagBuilder(HtmlTag.A);

                link.MergeAttribute(
                    HtmlAttribute.Href,
                    context.Get<ILinkBuilder>().GetLink(ForumPages.ActiveUsers, new { v = 1 }));
                link.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("COMMON", "VIEW_FULLINFO"));
                link.MergeAttribute("data-bs-toggle", "tooltip");

                if (context.IsCrawler)
                {
                    link.MergeAttribute(HtmlAttribute.Rel, "nofollow");
                }

                link.InnerHtml.Append(
                    context.Get<ILocalization>().GetTextFormatted(
                        activeMembers == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2",
                        activeMembers));

                content.AppendHtml(link);
            }
            else
            {
                content.Append(
                    context.Get<ILocalization>().GetTextFormatted(
                        activeMembers == 1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2",
                        activeMembers));
            }
        }

        if (activeGuests > 0)
        {
            if (canViewActive && (context.BoardSettings.ShowGuestsInDetailedActiveList ||
                                  context.BoardSettings.ShowCrawlersInActiveList))
            {
                content.Append(", ");

                var link = new TagBuilder(HtmlTag.A);

                link.MergeAttribute(
                    HtmlAttribute.Href,
                    context.Get<ILinkBuilder>().GetLink(ForumPages.ActiveUsers, new { v = 2 }));
                link.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("COMMON", "VIEW_FULLINFO"));
                link.MergeAttribute("data-bs-toggle", "tooltip");

                if (context.IsCrawler)
                {
                    link.MergeAttribute(HtmlAttribute.Rel, "nofollow");
                }

                link.InnerHtml.Append(
                    context.Get<ILocalization>().GetTextFormatted(
                        activeGuests == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2",
                        activeGuests));

                content.AppendHtml(link);
            }
            else
            {
                content.Append(
                    $", {context.Get<ILocalization>().GetTextFormatted(activeGuests == 1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2", activeGuests)}");
            }
        }

        if (activeHidden > 0 && context.IsAdmin)
        {
            content.Append(", ");

            var link = new TagBuilder(HtmlTag.A);

            link.MergeAttribute(HtmlAttribute.Href, context.Get<ILinkBuilder>().GetLink(ForumPages.ActiveUsers, new { v = 3 }));
            link.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("COMMON", "VIEW_FULLINFO"));
            link.MergeAttribute("data-bs-toggle", "tooltip");

            link.InnerHtml.Append(
                context.Get<ILocalization>().GetTextFormatted("ACTIVE_USERS_HIDDEN", activeHidden));

            content.AppendHtml(link);
        }

        content.Append(
            $" {context.Get<ILocalization>().GetTextFormatted("ACTIVE_USERS_TIME", context.BoardSettings.ActiveListTime)}");

        return content;
    }
}