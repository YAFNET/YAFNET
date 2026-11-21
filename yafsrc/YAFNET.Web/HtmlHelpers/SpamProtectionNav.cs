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
/// The tabs of the spam protection pages (only the pages the admin can open, like the admin menu).
/// </summary>
public static class SpamProtectionNavHtmlHelper
{
    /// <summary>
    /// The spam protection pages (the order of the admin menu) with the text of the admin menu and the icon.
    /// </summary>
    private static readonly (ForumPages Page, string Tag, string Icon)[] SpamPages =
    [
        (ForumPages.Admin_SpamLog, "admin_spamlog", "book"),
        (ForumPages.Admin_SpamWords, "admin_SpamWords", "comment-slash"),
        (ForumPages.Admin_BannedEmails, "admin_BannedEmail", "at"),
        (ForumPages.Admin_BannedIps, "admin_BannedIp", "network-wired"),
        (ForumPages.Admin_BannedNames, "admin_BannedName", "user-slash"),
        (ForumPages.Admin_BannedUserAgents, "ADMIN_BANNED_USERAGENTS", "user-secret"),
        (ForumPages.Admin_BannedCountries, "ADMIN_BANNED_COUNTRIES", "globe")
    ];

    /// <summary>
    /// Renders the tabs of the spam protection pages.
    /// </summary>
    /// <param name="_">The html helper.</param>
    /// <returns>The tabs.</returns>
    public static IHtmlContent SpamProtectionNav(this IHtmlHelper _)
    {
        var context = BoardContext.Current;

        var pagesAccess = context.Get<IDataCache>().GetOrSet(
            string.Format(Constants.Cache.AdminPageAccess, context.PageUserID),
            () => context.GetRepository<AdminPageUserAccess>().List(context.PageUserID).ToList());

        var list = new TagBuilder(HtmlTag.Ul);

        list.AddCssClass("nav nav-pills admin-spam-nav");

        foreach (var (page, tag, icon) in SpamPages)
        {
            if (!context.PageUser.UserFlags.IsHostAdmin && !pagesAccess.Exists(x => x.PageName == page.ToString()))
            {
                continue;
            }

            var isActive = context.CurrentForumPage.PageName == page;

            var link = new TagBuilder(HtmlTag.A);

            link.AddCssClass(isActive ? "nav-link active" : "nav-link");
            link.MergeAttribute(HtmlAttribute.Href, context.Get<ILinkBuilder>().GetLink(page));

            if (isActive)
            {
                link.MergeAttribute("aria-current", "page");
            }

            var iconTag = new TagBuilder(HtmlTag.I);

            iconTag.AddCssClass($"fas fa-{icon} me-1");
            iconTag.TagRenderMode = TagRenderMode.Normal;

            link.InnerHtml.AppendHtml(iconTag);
            link.InnerHtml.Append(context.Get<ILocalization>().GetText("ADMINMENU", tag));

            var item = new TagBuilder(HtmlTag.Li);

            item.AddCssClass("nav-item");
            item.InnerHtml.AppendHtml(link);

            list.InnerHtml.AppendHtml(item);
        }

        return list;
    }
}
