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
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
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
            CodeContracts.VerifyNotNull(pageLinks);

            pageLinks.AddLink(
                pageLinks.PageBoardContext.BoardSettings.Name,
                BoardContext.Current.Get<LinkBuilder>().GetLink(ForumPages.Board));

            return pageLinks;
        }

        /// <summary>
        /// The add admin index.
        /// </summary>
        /// <param name="pageLinks">
        /// The page links.
        /// </param>
        /// <returns>
        /// The <see cref="PageLinks"/>.
        /// </returns>
        public static PageLinks AddAdminIndex(this PageLinks pageLinks)
        {
            CodeContracts.VerifyNotNull(pageLinks);

            pageLinks.AddLink(
                pageLinks.GetText("ADMIN_ADMIN", "Administration"),
                BoardContext.Current.Get<LinkBuilder>().GetLink(ForumPages.Admin_Admin));

            return pageLinks;
        }

        /// <summary>
        /// The add user.
        /// </summary>
        /// <param name="pageLinks">
        /// The page links.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="PageLinks"/>.
        /// </returns>
        public static PageLinks AddUser(this PageLinks pageLinks, [NotNull] int userId, [NotNull] string name)
        {
            CodeContracts.VerifyNotNull(pageLinks);
            CodeContracts.VerifyNotNull(name);

            pageLinks.AddLink(name, BoardContext.Current.Get<LinkBuilder>().GetUserProfileLink(userId, name));

            return pageLinks;
        }

        /// <summary>
        /// The add topic.
        /// </summary>
        /// <param name="pageLinks">
        /// The page links.
        /// </param>
        /// <param name="topicName">
        /// The topic name.
        /// </param>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <returns>
        /// The <see cref="PageLinks"/>.
        /// </returns>
        public static PageLinks AddTopic(this PageLinks pageLinks, [NotNull] string topicName, [NotNull] int topicId)
        {
            CodeContracts.VerifyNotNull(pageLinks);

            pageLinks.AddLink(topicName, BoardContext.Current.Get<LinkBuilder>().GetTopicLink(topicId, topicName));

            return pageLinks;
        }

        /// <summary>
        /// Adds the category.
        /// </summary>
        /// <param name="pageLinks">
        /// The page links.
        /// </param>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <returns>
        /// Returns the Page links including the Category
        /// </returns>
        public static PageLinks AddCategory(
            this PageLinks pageLinks,
            [NotNull] Category category)
        {
            CodeContracts.VerifyNotNull(pageLinks);
            CodeContracts.VerifyNotNull(category);

            pageLinks.AddLink(
                category.Name,
                BoardContext.Current.Get<LinkBuilder>().GetCategoryLink(category.ID, category.Name));

            return pageLinks;
        }

        /// <summary>
        /// Adds the forum links.
        /// </summary>
        /// <param name="pageLinks">
        /// The page links.
        /// </param>
        /// <param name="forum">
        /// The forum.
        /// </param>
        /// <param name="noForumLink">
        /// The no forum link.
        /// </param>
        /// <returns>
        /// Returns the page links
        /// </returns>
        public static PageLinks AddForum(this PageLinks pageLinks, Forum forum, bool noForumLink = false)
        {
            CodeContracts.VerifyNotNull(pageLinks);

            if (forum.ParentID.HasValue)
            {
                var parent = BoardContext.Current.GetRepository<Forum>()
                    .GetById(forum.ParentID.Value);

                if (parent != null)
                {
                    pageLinks.AddLink(
                        parent.Name,
                        BoardContext.Current.Get<LinkBuilder>().GetForumLink(parent.ID, parent.Name));
                }
            }

            if (BoardContext.Current.PageForumID == forum.ID)
            {
                pageLinks.AddLink(
                    BoardContext.Current.PageForum.Name,
                    noForumLink
                        ? string.Empty
                        : BoardContext.Current.Get<LinkBuilder>().GetForumLink(
                            forum.ID,
                            BoardContext.Current.PageForum.Name));
            }

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
            CodeContracts.VerifyNotNull(pageLinks);
            CodeContracts.VerifyNotNull(title);

            pageLinks.Add(new PageLink { Title = title.Trim(), URL = url?.Trim() });

            return pageLinks;
        }
    }
}