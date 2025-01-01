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
/// The active users html helper.
/// </summary>
public static class ActiveUsersHtmlHelper
{
    /// <summary>
    /// The active users.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="activeUsersList">
    /// The active users list.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent ActiveUsers(
        this IHtmlHelper htmlHelper,
        List<ActiveUser> activeUsersList)
    {
        var content = new HtmlContentBuilder();

        var context = BoardContext.Current;

        // we shall continue only if there are active user data available
        if (!activeUsersList.HasItems())
        {
            return content;
        }

        // writes starting tag
        var list = new TagBuilder(HtmlTag.Ul);

        list.AddCssClass("list-inline");

        var crawlers = new List<string>();

        // go through the table and process each row
        activeUsersList.ForEach(
            user =>
                {
                    var listItem = new TagBuilder(HtmlTag.Li);
                    listItem.AddCssClass("list-inline-item");

                    IHtmlContent userLink;
                    IHtmlContent postFixContent = null;

                    // create new link and set its parameters
                    if (user.ActiveFlags.IsCrawler)
                    {
                        if (crawlers.Contains(user.Browser))
                        {
                            return;
                        }

                        crawlers.Add(user.Browser);

                        userLink = htmlHelper.UserLink(user.UserID, user.Browser);
                    }
                    else
                    {
                        userLink = htmlHelper.UserLink(
                            user.UserID,
                            context.BoardSettings.EnableDisplayName ? user.UserDisplayName : user.UserName,
                            user.Suspended,
                            user.UserStyle);
                    }

                    // how many users of this type is present (valid for guests, others have it 1)
                    var userCount = user.UserCount;
                    if (userCount > 1 && (!user.ActiveFlags.IsCrawler || !context.BoardSettings.ShowCrawlersInActiveList))
                    {
                        // add postfix if there is more the one user of this name
                        postFixContent = new HtmlContentBuilder().Append($" ({userCount})");
                    }

                    // if user is guest and guest should be hidden
                    var addControl = !(user.IsGuest && !context.IsAdmin);

                    // we might not want to add this user link if user is marked as hidden
                    if (user.IsActiveExcluded)
                    {
                        // hidden user are always visible to admin and himself
                        if (context.IsAdmin || user.UserID == context.PageUserID)
                        {
                            var icon = new TagBuilder(HtmlTag.I);
                            icon.AddCssClass("fas fa-user-secret ms-1");

                            postFixContent = icon;
                        }
                        else
                        {
                            // user is hidden from this user...
                            addControl = false;
                        }
                    }

                    // add user link if it's not suppressed
                    if (!addControl)
                    {
                        return;
                    }

                    listItem.InnerHtml.AppendHtml(userLink);

                    if (postFixContent is not null)
                    {
                        listItem.InnerHtml.AppendHtml(postFixContent);
                    }

                    list.InnerHtml.AppendHtml(listItem);
                });

        return content.AppendHtml(list);
    }
}