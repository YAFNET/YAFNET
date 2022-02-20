/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Web.Extensions
{
    using System.Linq;
    using System.Text;

    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Helpers;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Web.Controls;

    public static class ForumPageExtensions
    {
        public static string GeneratePageTitle(this ForumPage page)
        {
            var title = new StringBuilder();

            var pageString = string.Empty;

            if (BoardContext.Current.CurrentForumPage.PageType is ForumPages.Posts or ForumPages.Topics)
            {
                // get current page...
                var currentPager = page.FindControlAs<Pager>("Pager");

                if (currentPager != null && currentPager.CurrentPageIndex != 0)
                {
                    pageString = BoardContext.Current.BoardSettings.PagingTitleTemplate.Replace(
                        "{paging}",
                        $"{BoardContext.Current.Get<ILocalization>().GetText("COMMON", "PAGE")} {currentPager.CurrentPageIndex + 1}");
                }
            }

            if (!BoardContext.Current.CurrentForumPage.IsAdminPage)
            {
                switch (BoardContext.Current.CurrentForumPage.PageType)
                {
                    case ForumPages.Posts:
                        if (BoardContext.Current.PageTopicID != 0)
                        {
                            // Tack on the topic we're viewing
                            title.Append(
                                BoardContext.Current.Get<IBadWordReplace>()
                                    .Replace(BoardContext.Current.PageTopic.TopicName.Truncate(80)));
                        }

                        // Append Current Page
                        title.Append(pageString);

                        break;
                    case ForumPages.Topics:
                        if (BoardContext.Current.PageForum != null && BoardContext.Current.PageForum.Name.IsSet())
                        {
                            // Tack on the forum we're viewing
                            title.Append(page.HtmlEncode(BoardContext.Current.PageForum.Name.Truncate(80)));
                        }

                        // Append Current Page
                        title.Append(pageString);

                        break;
                    case ForumPages.Board:
                        if (BoardContext.Current.PageCategory != null && BoardContext.Current.PageCategory.Name.IsSet())
                        {
                            // Tack on the forum we're viewing
                            title.Append(page.HtmlEncode(BoardContext.Current.PageCategory.Name.Truncate(80)));
                        }

                        break;
                    default:
                        var pageLinks = page.FindControlAs<PageLinks>("PageLinks");

                        var activePageLink = pageLinks?.PageLinkList?.FirstOrDefault(link => link.URL.IsNotSet());

                        if (activePageLink != null)
                        {
                            // Tack on the forum we're viewing
                            title.Append(page.HtmlEncode(activePageLink.Title.Truncate(80)));
                        }

                        break;
                }
            }
            else
            {
                var pageLinks = page.FindControlAs<PageLinks>("PageLinks");

                var activePageLink = pageLinks?.PageLinkList?.FirstOrDefault(link => link.URL.IsNotSet());

                if (activePageLink != null)
                {
                    // Tack on the forum we're viewing
                    title.Append(page.HtmlEncode(activePageLink.Title));
                }
            }

            var boardName = page.HtmlEncode(BoardContext.Current.BoardSettings.Name);

            return title.Length > 0
                    ? BoardContext.Current.BoardSettings.TitleTemplate.Replace("{title}", title.ToString())
                        .Replace("{boardName}", boardName)
                    : boardName;
        }
    }
}