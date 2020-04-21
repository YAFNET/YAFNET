/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Web.Controls;

    using Forum = YAF.Types.Models.Forum;

    /// <summary>
    /// The PageLink Extensions
    /// </summary>
    public static class PageLinkExtensions
    {
        /// <summary>
        /// Adds the root.
        /// </summary>
        /// <param name="pageLinks">The page links.</param>
        /// <returns>returns the Page links including the root</returns>
        public static PageLinks AddRoot(this PageLinks pageLinks)
        {
            CodeContracts.VerifyNotNull(pageLinks, "pageLinks");

            pageLinks.AddLink(pageLinks.Get<BoardSettings>().Name, BuildLink.GetLink(ForumPages.Board));

            return pageLinks;
        }

        /// <summary>
        /// Adds the category.
        /// </summary>
        /// <param name="pageLinks">The page links.</param>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns>Returns the Page links including the Category</returns>
        public static PageLinks AddCategory(
            this PageLinks pageLinks,
            [NotNull] string categoryName,
            [NotNull] int categoryId)
        {
            CodeContracts.VerifyNotNull(pageLinks, "pageLinks");
            CodeContracts.VerifyNotNull(categoryName, "categoryName");

            pageLinks.AddLink(categoryName, BuildLink.GetLink(ForumPages.Board, "c={0}", categoryId));

            return pageLinks;
        }

        /// <summary>
        /// Adds the link.
        /// </summary>
        /// <param name="pageLinks">The page links.</param>
        /// <param name="title">The title.</param>
        /// <param name="url">The URL.</param>
        /// <returns>Returns the page links</returns>
        public static PageLinks AddLink(this PageLinks pageLinks, [NotNull] string title, [CanBeNull] string url = "")
        {
            CodeContracts.VerifyNotNull(pageLinks, "pageLinks");
            CodeContracts.VerifyNotNull(title, "title");

            pageLinks.Add(new PageLink { Title = title.Trim(), URL = url?.Trim() });

            return pageLinks;
        }

        /// <summary>
        /// Adds the forum links.
        /// </summary>
        /// <param name="pageLinks">The page links.</param>
        /// <param name="forumId">The forum id.</param>
        /// <param name="noForumLink">The no forum link.</param>
        /// <returns>Returns the page links</returns>
        public static PageLinks AddForum(this PageLinks pageLinks, int forumId, bool noForumLink = false)
        {
            CodeContracts.VerifyNotNull(pageLinks, "pageLinks");

            if (BoardContext.Current.PageParentForumID.HasValue)
            {
                var parent = BoardContext.Current.GetRepository<Forum>()
                    .GetById(BoardContext.Current.PageParentForumID.Value);

                if (parent != null)
                {
                    pageLinks.AddLink(parent.Name, BuildLink.GetLink(ForumPages.Topics, "f={0}", parent.ID));
                }
            }

            if (BoardContext.Current.PageForumID == forumId)
            {
                pageLinks.AddLink(
                    BoardContext.Current.PageForumName,
                    noForumLink ? string.Empty : BuildLink.GetLink(ForumPages.Topics, "f={0}", forumId));
            }

            return pageLinks;
        }
    }
}