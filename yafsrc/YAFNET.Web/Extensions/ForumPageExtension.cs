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

namespace YAF.Web.Extensions;

using YAF.Core.BasePages;

/// <summary>
/// Class ForumPageExtensions.
/// </summary>
public static class ForumPageExtensions
{
    /// <summary>
    /// Generates the page title.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <returns>string.</returns>
    public static string GeneratePageTitle(this ForumPage page)
    {
        var title = new StringBuilder();

        var pageString = string.Empty;

        if (page.PageName is ForumPages.Posts or ForumPages.Topics && BoardContext.Current.PageIndex > 0)
        {
            // get current page...
            pageString = BoardContext.Current.BoardSettings.PagingTitleTemplate.Replace(
                "{paging}",
                $"{BoardContext.Current.Get<ILocalization>().GetText("COMMON", "PAGE")} {BoardContext.Current.PageIndex + 1}");
        }

        if (!page.IsAdminPage)
        {
            switch (page.PageName)
            {
                case ForumPages.Post:
                    if (BoardContext.Current.PageTopicID != 0)
                    {
                        // Tack on the topic we're viewing
                        title.Append(
                            BoardContext.Current.Get<IBadWordReplace>().Replace(
                                BoardContext.Current.PageTopic.TopicName.Truncate(80)));
                    }

                    break;
                case ForumPages.Posts:
                    if (BoardContext.Current.PageTopicID != 0)
                    {
                        // Tack on the topic we're viewing
                        title.Append(
                            BoardContext.Current.Get<IBadWordReplace>().Replace(
                                BoardContext.Current.PageTopic.TopicName.Truncate(80)));
                    }

                    // Append Current Page
                    title.Append(pageString);

                    break;
                case ForumPages.Topics:
                    if (BoardContext.Current.PageForum is not null && BoardContext.Current.PageForum.Name.IsSet())
                    {
                        // Tack on the forum we're viewing
                        title.Append(page.HtmlEncode(BoardContext.Current.PageForum.Name.Truncate(80)));
                    }

                    // Append Current Page
                    title.Append(pageString);

                    break;
                case ForumPages.Index:
                    if (BoardContext.Current.PageCategory is not null && BoardContext.Current.PageCategory.Name.IsSet())
                    {
                        // Tack on the forum we're viewing
                        title.Append(page.HtmlEncode(BoardContext.Current.PageCategory.Name.Truncate(80)));
                    }

                    break;
                case ForumPages.Profile_DeleteAccount:
                    {
                        title.Append(
                            BoardContext.Current.Get<ILocalization>().GetTextFormatted(
                                "DELETE_ACCOUNT",
                                "TITLE",
                                BoardContext.Current.PageUser.DisplayOrUserName()));
                    }

                    break;
                case ForumPages.Privacy:
                    {
                        title.Append(BoardContext.Current.Get<ILocalization>().GetText("COMMON", "PRIVACY_POLICY"));
                    }

                    break;
                case ForumPages.MyAccount:
                    {
                        title.Append(BoardContext.Current.Get<ILocalization>().GetText("ACCOUNT", "YOUR_ACCOUNT"));
                    }

                    break;
                case ForumPages.Moderate_UnapprovedPosts:
                case ForumPages.Moderate_ReportedPosts:
                case ForumPages.Moderate_Forums:
                    {
                        title.Append(page.HtmlEncode(BoardContext.Current.PageForum.Name));
                    }

                    break;
                case ForumPages.Moderate_Moderate:
                    {
                        title.Append(BoardContext.Current.Get<ILocalization>().GetText("MODERATE_DEFAULT", "MODERATEINDEX_TITLE"));
                    }

                    break;
                case ForumPages.MyFriends:
                    {
                        title.Append(BoardContext.Current.Get<ILocalization>().GetText("BUDDYLIST_TT"));
                    }

                    break;

                default:
                    title.Append(page.HtmlEncode(page.PageTitle));

                    break;
            }
        }
        else
        {
            title.Append(page.HtmlEncode(page.PageTitle));
        }

        var boardName = page.HtmlEncode(BoardContext.Current.BoardSettings.Name);

        return title.Length > 0
                   ? BoardContext.Current.BoardSettings.TitleTemplate.Replace("{title}", title.ToString())
                       .Replace("{boardName}", boardName)
                   : boardName;
    }
}