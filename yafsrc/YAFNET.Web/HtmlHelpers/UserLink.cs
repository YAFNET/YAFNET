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

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// The user link html helper.
/// </summary>
public static class UserLinkHtmlHelper
{
    /// <summary>
    /// The user link.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent UserLink(
        this IHtmlHelper htmlHelper,
        User user)
    {
        return UserLink(htmlHelper, user.ID, user.DisplayOrUserName(), user.Suspended, user.UserStyle);
    }

    /// <summary>
    /// The user link.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="crawler">
    /// The crawler.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent UserLink(
        this IHtmlHelper htmlHelper,
        int userId,
        string crawler)
    {
        return UserLink(htmlHelper, userId, null, null, string.Empty, false, false, false, string.Empty, crawler);
    }

    /// <summary>
    /// Render PageUser Link for the PageUser Link BBCode Module
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="replaceName">
    /// The replace name.
    /// </param>
    /// <param name="suspended">
    /// The suspended.
    /// </param>
    /// <param name="style">
    /// The style.
    /// </param>
    /// <param name="blankTarget">
    /// The blank target.
    /// </param>
    /// <param name="cssClass">
    /// The CSS class.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent UserLink(
        this IHtmlHelper htmlHelper,
        int userId,
        string replaceName,
        DateTime? suspended,
        string style,
        bool blankTarget,
        string cssClass)
    {
        return UserLink(htmlHelper, userId, replaceName, suspended, style, false, true, blankTarget, cssClass);
    }

    /// <summary>
    /// The user link.
    /// </summary>
    /// <param name="_">
    /// The html helper.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="replaceName">
    /// The replace name.
    /// </param>
    /// <param name="suspended">
    /// The suspended.
    /// </param>
    /// <param name="style">
    /// The style.
    /// </param>
    /// <param name="isGuest">
    /// The is guest.
    /// </param>
    /// <param name="enableHoverCard">
    /// The enable hover card.
    /// </param>
    /// <param name="blankTarget">
    /// The blank Target.
    /// </param>
    /// <param name="cssClassAppend">
    /// The CSS Class Append.
    /// </param>
    /// <param name="crawlerName">
    /// The crawler Name.
    /// </param>
    /// <param name="postFixText"></param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent UserLink(
        this IHtmlHelper _,
        int userId,
        string replaceName,
        DateTime? suspended,
        string style,
        bool isGuest = false,
        bool enableHoverCard = true,
        bool blankTarget = false,
        string cssClassAppend = "",
        string crawlerName = "",
        string postFixText = "")
    {
        var content = new HtmlContentBuilder();
        var context = BoardContext.Current;

        var displayName = replaceName;

        // Replace Name with Crawler Name if Set, otherwise use regular display name or Replace Name if set
        if (crawlerName.IsSet())
        {
            displayName = crawlerName;
        }

        if (userId == -1)
        {
            return content;
        }

        var cssClass = new StringBuilder();

        var isCrawler = crawlerName.IsSet();

        if (isCrawler)
        {
            isGuest = true;
        }

        if (!isGuest)
        {
            var link = new TagBuilder(HtmlTag.A);

            link.MergeAttribute(HtmlAttribute.Href, context.Get<LinkBuilder>().GetUserProfileLink(userId, displayName));

            cssClass.Append("btn-sm");

            if (context.Get<IPermissions>().Check(context.BoardSettings.ProfileViewPermissions) &&
                context.BoardSettings.EnableUserInfoHoverCards && enableHoverCard)
            {
                cssClass.Append(" hc-user");

                var userInfoLink = context.Get<IUrlHelper>().Action(
                    "GetUserInfo",
                    "UserInfo",
                    new {userId });

                link.MergeAttribute("data-hovercard", userInfoLink);
            }
            else
            {
                link.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("COMMON", "VIEW_USRPROFILE"));
            }

            if (cssClassAppend.IsSet())
            {
                cssClass.Append(cssClassAppend);
            }

            link.AddCssClass(cssClass.ToString());

            if (context.BoardSettings.UseNoFollowLinks)
            {
                link.MergeAttribute(HtmlAttribute.Rel, "nofollow");
            }

            if (style.IsSet() && context.BoardSettings.UseStyledNicks)
            {
                var styleFormatted = context.Get<IStyleTransform>().Decode(style);

                link.MergeAttribute(HtmlAttribute.Style, HttpUtility.HtmlEncode(styleFormatted));
            }

            if (blankTarget)
            {
                link.MergeAttribute(HtmlAttribute.Target, "_blank");
            }

            if (context.BoardSettings.ShowUserOnlineStatus)
            {
                link.InnerHtml.AppendHtml(RenderStatusIcon(context, userId, suspended));
            }

            link.InnerHtml.Append(displayName);

            if (postFixText.IsSet())
            {
                link.InnerHtml.AppendHtml(postFixText);
            }

            return content.AppendHtml(link);
        }
        else
        {
            var link = new TagBuilder(HtmlTag.Span);

            if (context.BoardSettings.ShowUserOnlineStatus)
            {
                link.InnerHtml.AppendHtml(RenderStatusIcon(context, userId, suspended));
            }

            link.InnerHtml.AppendHtml(displayName);

            return content.AppendHtml(link);
        }
    }

    /// <summary>
    ///  show online icon
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="suspended">
    /// The suspended.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    private static TagBuilder RenderStatusIcon(BoardContext context, int userId, DateTime? suspended)
    {
        var activeUsers = context.Get<IDataCache>().GetOrSet(
            Constants.Cache.UsersOnlineStatus,
            () => context.GetRepository<Active>().List(
                context.BoardSettings.ShowCrawlersInActiveList,
                context.BoardSettings.ActiveListTime),
            TimeSpan.FromMilliseconds(BoardContext.Current.BoardSettings.OnlineStatusCacheTimeout));

        var status = new TagBuilder(HtmlTag.Span);

        if (suspended.HasValue)
        {
            // suspended
            status.AddCssClass("align-middle text-warning user-suspended me-1");
            status.MergeAttribute(
                HtmlAttribute.Title,
                context.Get<ILocalization>().GetTextFormatted("USERSUSPENDED", suspended.Value));
            status.MergeAttribute("data-bs-toggle", "tooltip");
        }
        else
        {
            if (activeUsers.Exists(x => x.UserID == userId && !x.IsActiveExcluded))
            {
                // online
                status.AddCssClass("align-middle text-success user-online me-1");
                status.MergeAttribute("data-bs-title", context.Get<ILocalization>().GetText("USERONLINESTATUS"));
                status.MergeAttribute("data-bs-toggle", "tooltip");
            }
            else
            {
                // offline
                status.AddCssClass("align-middle text-danger user-offline me-1");
                status.MergeAttribute("data-bs-title", context.Get<ILocalization>().GetText("USEROFFLINESTATUS"));
                status.MergeAttribute("data-bs-toggle", "tooltip");
            }
        }

        var icon = new TagBuilder(HtmlTag.I);

        icon.AddCssClass("fas fa-user-circle");
        icon.MergeAttribute(HtmlAttribute.Style, "font-size: 1.5em");

        status.InnerHtml.AppendHtml(icon);

        return status;
    }
}