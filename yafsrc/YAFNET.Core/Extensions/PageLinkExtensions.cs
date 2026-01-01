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

using System;

namespace YAF.Core.Extensions;

using System.Collections.Generic;

using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The PageLink Extensions
/// </summary>
public static class PageLinkExtensions
{
    /// <param name="pageLinks">The page links.</param>
    extension(List<PageLink> pageLinks)
    {
        /// <summary>
        /// Adds the root.
        /// </summary>
        /// <returns>returns the Page links including the root</returns>
        public List<PageLink> AddRoot()
        {
            ArgumentNullException.ThrowIfNull(pageLinks);

            pageLinks.AddLink(
                BoardContext.Current.BoardSettings.Name,
                BoardContext.Current.Get<ILinkBuilder>().GetLink(ForumPages.Index));

            return pageLinks;
        }

        /// <summary>
        /// Adds the index of the admin.
        /// </summary>
        /// <returns>
        /// Returns the page links.
        /// </returns>
        public List<PageLink> AddAdminIndex()
        {
            ArgumentNullException.ThrowIfNull(pageLinks);

            pageLinks.AddLink(
                BoardContext.Current.Get<ILocalization>().GetText("ADMIN_ADMIN", "Administration"),
                BoardContext.Current.Get<ILinkBuilder>().GetLink(ForumPages.Admin_Admin));

            return pageLinks;
        }

        /// <summary>
        /// Adds the user link to the page links.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// Returns the page links.
        /// </returns>
        public List<PageLink> AddUser(int userId, string name)
        {
            ArgumentNullException.ThrowIfNull(pageLinks);

            pageLinks.AddLink(name, BoardContext.Current.Get<ILinkBuilder>().GetUserProfileLink(userId, name));

            return pageLinks;
        }

        /// <summary>
        /// Adds the topic link to the page links.
        /// </summary>
        /// <param name="topic">
        /// The topic.
        /// </param>
        /// <returns>
        /// Returns the page links.
        /// </returns>
        public List<PageLink> AddTopic(Topic topic)
        {
            ArgumentNullException.ThrowIfNull(pageLinks);

            if (topic is null)
            {
                return pageLinks;
            }

            pageLinks.AddLink(topic.TopicName, BoardContext.Current.Get<ILinkBuilder>().GetTopicLink(topic));

            return pageLinks;
        }

        /// <summary>
        /// Adds the category link to the page links.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <returns>
        /// Returns the Page links including the Category
        /// </returns>
        public List<PageLink> AddCategory(Category category)
        {
            ArgumentNullException.ThrowIfNull(pageLinks);

            if (category is null)
            {
                return pageLinks;
            }

            pageLinks.AddLink(
                category.Name,
                BoardContext.Current.Get<ILinkBuilder>().GetCategoryLink(category.ID, category.Name));

            return pageLinks;
        }

        /// <summary>
        /// Adds the forum link to the page links.
        /// </summary>
        /// <param name="forum">
        /// The forum.
        /// </param>
        /// <param name="noForumLink">
        /// The no forum link.
        /// </param>
        /// <returns>
        /// Returns the page links
        /// </returns>
        public List<PageLink> AddForum(Forum forum, bool noForumLink = false)
        {
            ArgumentNullException.ThrowIfNull(pageLinks);

            if (forum is null)
            {
                return pageLinks;
            }

            if (forum.ParentID.HasValue)
            {
                var parent = BoardContext.Current.GetRepository<Forum>()
                    .GetById(forum.ParentID.Value);

                if (parent != null)
                {
                    pageLinks.AddLink(
                        parent.Name,
                        BoardContext.Current.Get<ILinkBuilder>().GetForumLink(parent));
                }
            }

            if (BoardContext.Current.PageForumID == forum.ID)
            {
                pageLinks.AddLink(
                    BoardContext.Current.PageForum.Name,
                    noForumLink
                        ? string.Empty
                        : BoardContext.Current.Get<ILinkBuilder>().GetForumLink(
                            BoardContext.Current.PageForum));
            }

            return pageLinks;
        }

        /// <summary>
        /// Adds new page link.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="url">The URL.</param>
        /// <returns>Returns the page links</returns>
        public List<PageLink> AddLink(string title, string url = "")
        {
            ArgumentNullException.ThrowIfNull(pageLinks);

            pageLinks.Add(new PageLink { Title = title.Trim(), URL = url?.Trim() });

            return pageLinks;
        }
    }
}